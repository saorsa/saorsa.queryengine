// ReSharper disable InconsistentNaming
namespace Saorsa.QueryEngine.Model;

public enum FilterOperatorType
{
    IS_NULL,
    IS_NOT_NULL,
    EQ,
    NOT_EQ,
    LT,
    LT_EQ,
    GT,
    GT_EQ,
    RANGE,
    SEQUENCE,
    CONTAINS,
    IS_EMPTY,
    IS_NOT_EMPTY,
}
