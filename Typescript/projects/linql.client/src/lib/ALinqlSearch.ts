import { LinqlConstant, LinqlExpression, LinqlFunction, LinqlSearch, LinqlType } from "linql.core";
import { AOrderedLinqlSearch } from "./AOrderedLinqlSearch";
import { IGrouping } from "./IGrouping";
import { AnyExpression, BooleanExpression, GenericConstructor, TransformExpression } from "./Types";


export declare type LinqlSearchConstructor<T> =
    new (
        Type: string | GenericConstructor<T>,
        ArgumentContext: {} | undefined,
        Context: ALinqlContext
    ) => ALinqlSearch<any>;


export abstract class ALinqlSearch<T> extends LinqlSearch
{
    public override Expressions: Array<LinqlExpression> | undefined;

    constructor(public ModelType: string | GenericConstructor<T>, public ArgumentContext: {} | undefined = {}, public Context: ALinqlContext)
    {
        super(new LinqlType());
    }

    abstract Copy(): this;

    public CustomLinqlFunction<S>(FunctionName: string, Expression: AnyExpression<T> | string | undefined): ALinqlSearch<S>
    {

        let expressions = new Array<LinqlExpression>();
        if (this.Expressions)
        {
            expressions = Array.from(this.Expressions);
        }

        const newExpression = this.Context.Parse(Expression);
        const newSearch = this.Copy();

        const firstExpression = this.Expressions?.find(r => true);

        if (firstExpression)
        {
            const lastExpression = firstExpression.GetLastExpressionInNextChain();
            lastExpression.Next = newExpression;
        }
        else
        {
            expressions.push(newExpression);
        }
        newSearch.Expressions = expressions;
        return newSearch as any as ALinqlSearch<S>;
    }

    //#region Functions

    public filter(Expression: BooleanExpression<T> | string)
    {
        return this.CustomLinqlFunction<T>("Where", Expression);
    }

    public any(Expression: BooleanExpression<T> | string)
    {
        return this.CustomLinqlFunction<T>("Any", Expression);
    }

    public all(Expression: BooleanExpression<T> | string)
    {
        return this.CustomLinqlFunction<T>("All", Expression);
    }

    public distinct()
    {
        return this.CustomLinqlFunction<T>("Distinct", undefined);
    }

    public select<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<S>("Select", Expression);
    }

    public selectMany<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<S>("SelectMany", Expression);
    }

    public groupBy<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<IGrouping<S, T>>("GroupBy", Expression);
    }

    public orderBy<S>(Expression: TransformExpression<T, S>): AOrderedLinqlSearch<T>
    {
        return this.CustomLinqlFunction<T>("OrderBy", Expression) as AOrderedLinqlSearch<T>;
    }

    public orderByDescending<S>(Expression: TransformExpression<T, S>): AOrderedLinqlSearch<T>
    {
        return this.CustomLinqlFunction<T>("OrderByDescending", Expression) as AOrderedLinqlSearch<T>;
    }

    public skip(Skip: number)
    {
        const type = new LinqlType();
        type.TypeName = "Int32";
        const constant = new LinqlConstant(type, Skip);
        const fun = new LinqlFunction("Skip", [constant]);
        const newExpression = this.Copy();
        newExpression.Expressions?.push(fun);
        return newExpression;
    }

    public take(Take: number)
    {
        const type = new LinqlType();
        type.TypeName = "Int32";
        const constant = new LinqlConstant(type, Take);
        const fun = new LinqlFunction("Take", [constant]);
        const newExpression = this.Copy();
        newExpression.Expressions?.push(fun);
        return newExpression;
    }

    toJson(): string
    {
        return this.Context.ToJson(this);
    }

    //#endregion

    //#region ExecuteFunctions

    async executeCustomLinqlFunction<TResult>(FunctionName: string, Predicate: AnyExpression<any> | string | undefined): Promise<TResult>
    {
        const search = this.CustomLinqlFunction(FunctionName, Predicate);
        return await this.Context.GetResult<TResult>(search);
    }

    async toListAsync()
    {
        return null;
    }
    //#endregion



}

export abstract class ALinqlContext
{
    constructor(public LinqlSearchType: LinqlSearchConstructor<any>, public ArgumentContext: {} | undefined)
    {

    }

    public abstract GetResult<TResult>(Search: ALinqlSearch<any>): Promise<TResult>;

    protected abstract SendHttpRequest<TResult>(Endpoint: string, Search: ALinqlSearch<any>): Promise<TResult>;

    protected abstract SendRequest<TResult>(Search: ALinqlSearch<any>): Promise<TResult>;

    ToJson(Search: ALinqlSearch<any>)
    {
        return JSON.stringify(Search);
    }

    protected GetEndpoint(Search: ALinqlSearch<any>)
    {
        let endPoint: string;
        if (typeof Search.ModelType === "string")
        {
            endPoint = Search.ModelType;
        }
        else
        {
            endPoint = Search.ModelType.name;
        }
        return `linql/${ endPoint }`
    }

    public abstract Parse(Expression: any): LinqlExpression;

    public Set<T>(Type: string | GenericConstructor<T>, ArgumentContext: {} | undefined = this.ArgumentContext): ALinqlSearch<T>
    {
        return new this.LinqlSearchType(Type, ArgumentContext, this);
    }
}