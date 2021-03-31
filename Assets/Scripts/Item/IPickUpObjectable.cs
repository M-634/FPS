namespace Musashi
{
    /// <summary>
    /// 武器やアイテムの中で拾って使うもの
    /// </summary>
    public interface IPickUpObjectable
    {
        void OnPicked();
        void Drop();
    }
}