namespace KC
{
    public interface IRoot
    {
        Root ParentRoot { get; set; }

        void Dispose();
    }
}