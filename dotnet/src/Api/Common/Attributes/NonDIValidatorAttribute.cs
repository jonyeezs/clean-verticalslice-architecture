namespace CleanSlice.Api.Common.Attributes
{
    /// <summary>
    /// This allows this FluentValidator to be ignored for Dependency Injection
    /// Perhaps there are Validators that you want to use at runtime with arguments in the constructor not used for DI
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NonInjectableValidatorAttribute : Attribute { }
}
