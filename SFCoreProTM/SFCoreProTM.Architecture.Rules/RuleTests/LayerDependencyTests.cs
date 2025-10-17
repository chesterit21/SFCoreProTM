using System.Reflection;
using NetArchTest.Rules;
using SFCoreProTM.Application.Interfaces;
using SFCoreProTM.Domain.Entities.Issues;
using SFCoreProTM.Persistence.Data;
using SFCoreProTM.Presentation.Controllers;
using Xunit;

namespace SFCoreProTM.Architecture.Rules.RuleTests
{
    public class LayerDependencyTests
    {
        private const string DomainNamespace = "SFCoreProTM.Domain";
        private const string ApplicationNamespace = "SFCoreProTM.Application";
        private const string PersistenceNamespace = "SFCoreProTM.Persistence";
        private const string PresentationNamespace = "SFCoreProTM.Presentation";

        // Dapatkan semua Assembly dari class tertentu di setiap proyek
        private static readonly Assembly DomainAssembly = typeof(Issue).Assembly;
        private static readonly Assembly ApplicationAssembly = typeof(IUnitOfWork).Assembly;
        private static readonly Assembly PersistenceAssembly = typeof(ApplicationDbContext).Assembly;
        private static readonly Assembly PresentationAssembly = typeof(IssuesController).Assembly;

        [Fact]
        public void Domain_Layer_Should_Not_Have_Dependency_On_Other_Layers()
        {
        var result = Types
            .InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOn(ApplicationNamespace)
            .And()
            .NotHaveDependencyOn(PersistenceNamespace)
            .GetResult();

            Assert.True(result.IsSuccessful, "Domain Layer memiliki dependensi yang dilarang.");
        }

        [Fact]
        public void Controllers_Should_Not_Directly_Reference_Persistence()
        {
            // Hanya class yang berada di namespace 'Persistence' yang boleh mereferensikan DbContext.
            // Kita cek semua Controllers di layer API.
        var result = Types
            .InAssembly(PresentationAssembly)
            .That()
            .ResideInNamespace(PresentationNamespace + ".Controllers")
            .Should()
            .NotHaveDependencyOn(PersistenceNamespace)
            .GetResult();

            Assert.True(
                result.IsSuccessful,
                "Controllers dilarang memiliki dependensi langsung ke Persistence Layer."
            );
        }
    }
}
