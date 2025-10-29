using Linql.Core.Test;
using Linql.Test.Files;

namespace Linql.Client.Test.Expressions
{
    public class SimpleExpressions_Test : TestFileTests
    {
        protected LinqlContext Context { get; set; } = new LinqlContext(null, new JsonSerializerOptions() { 
            WriteIndented = true, 
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull 
        });

        protected override string TestFolder { get; set; } = "./SimpleExpressions";

        [Test]
        public async Task LinqlConstant()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => false).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions_Test.LinqlConstant), output);
        }

        [Test]
        public async Task LinqlBinary()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => r.Boolean && r.OneToOne.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions_Test.LinqlBinary), output);
        }

        [Test]
        public async Task LinqlUnary()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => !r.Boolean).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions_Test.LinqlUnary), output);
        }

        [Test]
        public async Task LinqlObject()
        {
            LinqlObject<DataModel> objectTest = new LinqlObject<DataModel>(new DataModel());
            Assert.That(objectTest.TypedValue, Is.EqualTo(objectTest.Value));
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => objectTest.TypedValue.Integer == r.Integer).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions_Test.LinqlObject), output);

        }

        [Test]
        public async Task LinqlFunction()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => r.ListInteger.Contains(1)).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions_Test.LinqlFunction), output);

        }

        [Test]
        public async Task LinqlLambda()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => r.ListInteger.Any(s => s > 0)).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions_Test.LinqlLambda), output);

        }


        [Test]
        public async Task FunctionChaining()
        {
            bool test = false;
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => false).Where(r => true).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions_Test.FunctionChaining), output);
        }

        [Test]
        public async Task NullableCheck()
        {
            bool test = false;
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions_Test.NullableCheck), output);
        }

        //[Test]
        //public async Task ToList()
        //{
        //    bool test = false;
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string output = await search.Where(r => r.OneToOneNullable.Integer.HasValue && r.OneToOneNullable.Integer.Value == 1).ToLinqlList().ToJsonAsync();
        //    this.TestLoader.Compare(nameof(SimpleExpressions.ToList), output);
        //}

        [Test]
        public void ExecuteToList()
        {
            Assert.Catch(() =>
            {
                bool test = false;
                LinqlSearch<DataModel> search = Context.Set<DataModel>();
                List<DataModel> output = search.AsQueryable().ToList();
            });
        }

        [Test]
        public async Task AnonymousObject()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();

            var search2 = search.Select(r => new
            {
                Property1 = r.Boolean,
                Property2 = r.Decimal
            });

            string output = await search2.ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions_Test.AnonymousObject), output);
        }

        //Not implemented yet.  May not be possible
        //[Test]
        //public async Task AnonymousObjectConcreteType()
        //{
        //    LinqlSearch<DataModel> search = Context.Set<DataModel>();
        //    string output = await search.Select(r => new TestProjection(r.Boolean, r.Decimal)).ToJsonAsync();
        //    this.TestLoader.Compare(nameof(SimpleExpressions_Test.AnonymousObjectConcreteType), output);
        //}

        [Test]
        public async Task AnonymousObjectChained()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            string output = await search.Select(r => new
            {
                Property1 = r.Boolean,
                Property2 = r.Decimal
            }.Property1).ToJsonAsync();
            this.TestLoader.Compare(nameof(SimpleExpressions_Test.AnonymousObjectChained), output);
        }

        [Test]
        public async Task GroupBy()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();

            string output = await search.GroupBy(r => r.Boolean).Select(r => new
            {
                key = r.Key,
                count = r.Count()
            }).ToJsonAsync();

            this.TestLoader.Compare(nameof(SimpleExpressions_Test.GroupBy), output);
        }

        [Test]
        public async Task GroupByString()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();

            string output = await search.GroupBy(r => r.Boolean).Select(r => new
            {
                key = r.Key,
                count = String.Join(",", r.Select(s => s.String))
            }).ToJsonAsync();

            this.TestLoader.Compare(nameof(SimpleExpressions_Test.GroupByString), output);
        }

        [Test]
        public async Task MultipleQueries()
        {
            LinqlSearch<DataModel> search = Context.Set<DataModel>();
            LinqlSearch<DataModel> search2 = Context.Set<DataModel>();

            var innerQuery = search2.Select(r => r.Integer);

            string output = await search.Where(r => search2.Select(s => s.Integer).Contains(r.Integer)).ToJsonAsync();

            this.TestLoader.Compare(nameof(SimpleExpressions_Test.MultipleQueries), output);
        }
    }

    class TestProjection
    {
        public bool Property1 { get; set; }

        public decimal Property2 { get; set; }

        public TestProjection(bool Property1, decimal Property2)
        {
            this.Property1 = Property1;
            this.Property2 = Property2;
        }
    }
}