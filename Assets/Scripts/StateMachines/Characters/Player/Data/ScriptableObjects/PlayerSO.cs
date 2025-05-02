using UnityEngine;

namespace Cyber
{
    [CreateAssetMenu(fileName = "Player", menuName = "Cyber/Characters/Player")]
    public class PlayerSO : ScriptableObject
    {
        [field: SerializeField] public PlayerGroundedData GroundedData { get; private set; }
        [field: SerializeField] public PlayerAirborneData AirborneData { get; private set; }
        [field: SerializeField] public PlayerAttackData AttackData { get; private set; }
        
    }
}