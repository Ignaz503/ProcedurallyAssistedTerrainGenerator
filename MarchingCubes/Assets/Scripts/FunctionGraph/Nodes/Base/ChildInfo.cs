public struct ChildInfo
{
    public readonly bool IsChild;
    public readonly int ChildIndex;

    public ChildInfo(bool isChild, int childIndex)
    {
        IsChild = isChild;
        ChildIndex = childIndex;
    }
}