namespace ScriptConverter.Parser
{
    enum PrecedenceValue
    {
        Invalid,
        Assignment,
        Ternary,
        LogicalOr,
        LogicalAnd,
        BitwiseOr,
        BitwiseXor,
        BitwiseAnd,
        Equality,
        Relational,
        BitwiseShift,
        Additive,
        Multiplicative,
        Cast,
        Prefix,
        Suffix
    }
}
