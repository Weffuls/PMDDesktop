namespace PMDDesktop.Exceptions;

public class MissingAttributeException(Type type, Type attributeType) : Exception($"{type.FullName} was expected to have the {attributeType.FullName} attribute!")
{

}
