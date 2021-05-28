namespace LineCompare.Comparators
{
    public interface IComparatorCreator
    {
        public string Name { get; }

        public IComparator Create(string firstFile, string secondFile);
    }
}