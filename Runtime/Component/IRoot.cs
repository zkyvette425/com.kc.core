namespace KC
{
    internal interface IRoot
    {
        Root ParentRoot { get; set; }
        
        int RootType { get; set; }
    }
}