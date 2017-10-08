using System.Collections.Generic;

public interface IEnemy
{
    bool ChasePlayer { set; get; }
    void Awaken();
    void Sleepen();
    void TakeDamage(int amount);
}