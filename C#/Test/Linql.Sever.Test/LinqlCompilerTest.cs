using Linql.Client;
using Linql.Core;
using Linql.Core.Test;
using Linql.Test.Files;
using System.Reflection;
using System.Text.Json;

namespace Linql.Server.Test
{
    public class LinqlCompilerTest : TestFileTests
    {
        public IQueryable<DataModel> Data { get; set; }

        public LinqlCompiler Compiler { get; set; } 

        [SetUp]
        public override async Task Setup()
        {
            await base.Setup();
            List<DataModel> dataList = new List<DataModel>();
            dataList.Where(r => true).ToList();
            foreach (int index in Enumerable.Range(1, 100))
            {
                DataModel data = new CompiledDataModel(index, true);
                dataList.Add(data);
            }

            Data = dataList.AsQueryable();

            List<Assembly> assemblies = new List<Assembly>() 
            { 
                typeof(Boolean).Assembly, 
                //typeof(Enumerable).Assembly, 
                typeof(Queryable).Assembly 
            };
            this.Compiler = new LinqlCompiler(assemblies);


        }

        //[Test]
        //public void Constructor()
        //{
        //    //string json = this.TestLoader.TestFiles["Function"];
        //    //LinqlExpression? search = JsonSerializer.Deserialize<LinqlExpression>(json);

        //    string json = this.TestLoader.TestFiles.LastOrDefault().Value;
        //    LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

        //    Assert.DoesNotThrow(() =>
        //    {
        //        if (search != null)
        //        {
        //            LinqlCompiler compiler = new LinqlCompiler(search);
        //        }
        //    });
        //}

        //[Test]
        //public void SearchIsSet()
        //{
        //    string json = this.TestLoader.TestFiles.LastOrDefault().Value;
        //    LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

        //    Assert.DoesNotThrow(() =>
        //    {
        //        if (search != null)
        //        {
        //            DerivedCompiler compiler = new DerivedCompiler(search);
        //            Assert.NotNull(compiler.GetSearch());
        //        }
        //    });
        //}

        [Test]
        public void WhereFalse()
        {           
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            Assert.DoesNotThrow(() =>
            {
                IEnumerable<DataModel> data = this.Compiler.Execute<IEnumerable<DataModel>>(search, this.Data);

                Assert.That(data.Count(), Is.EqualTo(0));
            });
        }

        [Test]
        public void WhereFalseLinqlSearch()
        {
            string json = this.TestLoader.TestFiles["SimpleBooleanFalse"];
            LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);

            LinqlSearch<DataModel> data = new LinqlSearch<DataModel>();

            Assert.DoesNotThrow(() =>
            {
                IEnumerable<DataModel> result = this.Compiler.Execute<IEnumerable<DataModel>>(search, data);
                string test = result.AsQueryable().ToJson();

            });
        }
    }
}