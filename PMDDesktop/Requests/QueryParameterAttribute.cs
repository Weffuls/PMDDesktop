namespace PMDDesktop.Requests;

/// <summary>
/// Attaching this property to a ServerRequest moves the data to the url during the request, then moves it back on parse. This can be useful for adding parameters to GET requests and caching data based on URL.
/// </summary>
/// <remarks>
/// This only works on properties that are in the first layer of the object. Properties inside sub-objects of the JSON will not be moved to the url.
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public class QueryParameterAttribute : Attribute
{



}
