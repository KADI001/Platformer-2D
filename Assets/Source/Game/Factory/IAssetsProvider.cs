namespace Source
{
    public interface IAssetsProvider
    {
        Enemy GetEnemy();
        Player GetPlayer();
        Bullet GetBullet();
    }
}