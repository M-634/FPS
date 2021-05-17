namespace Musashi
{
    /// <summary>
    /// ダメージを受けるオブジェット
    /// </summary>
    public interface IDamageable
    {
        void OnDamage(float damage);
        TargetType GetTargetType();
    }

    /// <summary>
    /// ダメージを受けるオブジェットのタイプ
    /// </summary>
    public enum TargetType
    {
        Defult,//当たってもエフェクトを出さない
        Humanoid,
        Mechanical,
    }
}
