namespace PMDDesktop.Exceptions;

public class DuplicateAttributeDataException(Type type1, Type type2, object overlappingData, Type attributeType) : Exception($"{type1.FullName} and {type2.FullName} have overlapping data '{overlappingData}' on {attributeType.FullName}!")
{

}
