using System.Linq;
using CsToTs.Definitions;
using CsToTs.Tests.Fixture;
using Xunit;

namespace CsToTs.Tests {

    public class ReflectionTests {

        [Fact]
        public void ShouldHandleOpenAndClosedGenericInterface() {
            // should only generate generic type definitions
            foreach (var t in new[] {typeof(IBase<>), typeof(IBase<int>)}) {
                var res = Generator.GetTypeDefinitions(t);

                var types = res.Types.ToList();
                Assert.Equal(1, types.Count());

                var ibase = types.First();
                Assert.Equal(1, ibase.GenericParameters.Count);

                var gen = ibase.GenericParameters.First();
                Assert.False(gen.HasNewConstraint);
                Assert.Equal("T", gen.Name);
                Assert.Equal(0, gen.TypeConstraints.Count);

                var mem = ibase.Members;
                Assert.Equal(1, mem.Count);

                var id = mem.First();
                Assert.Equal("Id", id.Name);
                
                Assert.Equal(0, res.Enums.Count);
            }
        }

        [Fact]
        public void ShouldHandleOpenAndClosedGenericType() {
            // should only generate generic type definitions
            foreach (var t in new[] {typeof(BaseEntity<>), typeof(BaseEntity<int>)}) {
                var res = Generator.GetTypeDefinitions(t);

                var types = res.Types;
                Assert.Equal(2, types.Count());

                // interface should be first, so we take the last type definition
                var bas = types.Last();
                Assert.Equal(1, bas.GenericParameters.Count);

                var gen = bas.GenericParameters.First();
                Assert.False(gen.HasNewConstraint);
                Assert.Equal("TKey", gen.Name);
                Assert.Equal(0, gen.TypeConstraints.Count);

                var inter = bas.ImplementedInterfaces;
                Assert.Equal(1, inter.Count);

                var ibase = inter.First();
                Assert.Equal("IBase", ibase.Name);

                var mem = bas.Members;
                Assert.Equal(6, mem.Count);

                var idMember = mem.FirstOrDefault(m => m.Name == "Id");
                Assert.NotNull(idMember);

                var typeMember = mem.FirstOrDefault(m => m.Name == "Type");
                Assert.NotNull(typeMember);
                
                var enums = res.Enums;
                Assert.Equal(1, enums.Count);

                var typeEnum = enums.First();
                Assert.Equal("TypeEnum", typeEnum.Name);

                var typeFields = typeEnum.Fields;
                Assert.Equal(2, typeFields.Count);

                var type1Field = typeFields.ElementAt(0);
                Assert.Equal("Type1", type1Field.Name);
                Assert.Equal(2, type1Field.Value);

                var type2Field = typeFields.ElementAt(1);
                Assert.Equal("Type2", type2Field.Name);
                Assert.Equal(5, type2Field.Value);
            }
        }
    }
}