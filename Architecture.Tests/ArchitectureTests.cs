using NetArchTest.Rules;
using System.Reflection;

namespace Architecture.Tests;

public class ArchitectureTests
{
    private const string ApiNamespace = "Gameapp.Api";
    private const string ApplicationNamespace = "Gameapp.Application";
    private const string DomainNamespace = "Gameapp.Domain";
    private const string InfrastructureNamespace = "Gameapp.Infrastructure";

    [Fact]
    public void Application_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        Assembly assembly = Assembly.Load("Gameapp.Application");
        string[] otherProject = new[]
        {
            ApiNamespace,
            InfrastructureNamespace
        };

        // Act
        TestResult testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProject)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    [Fact]
    public void Domain_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        Assembly assembly = Assembly.Load("Gameapp.Domain");
        string[] otherProject = new[]
        {
            ApiNamespace, 
            ApplicationNamespace, 
            InfrastructureNamespace
        };

        // Act
        TestResult testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProject)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    [Fact]
    public void Infrastructure_Should_Not_HaveDependencyOnOtherProjects()
    {
        // Arrange
        Assembly assembly = Assembly.Load("Gameapp.Infrastructure");
        string[] otherProject = new[]
        {
            ApiNamespace
        };

        // Act
        TestResult testResult = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAll(otherProject)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    [Fact]
    public void Handlers_Should_Have_DependencyOnDomain()
    {
        // Arrange
        Assembly assembly = Assembly.Load("Gameapp.Application");

        // Act
        TestResult testResult = Types.InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Handler")
            .Should()
            .HaveDependencyOn(DomainNamespace)
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }

    [Fact]
    public void Controllers_Should_HaveDependencyOnMediatR()
    {
        // Arrange
        Assembly assembly = Assembly.Load("Gameapp.Api");

        // Act
        TestResult testResult = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .Should()
            .HaveDependencyOn("MediatR")
            .GetResult();

        // Assert
        Assert.True(testResult.IsSuccessful);
    }
}
