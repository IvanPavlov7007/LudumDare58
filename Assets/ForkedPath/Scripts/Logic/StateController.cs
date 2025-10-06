using System.Collections;
using UnityEngine;

public class StateController : MonoBehaviour
{
    public abstract IState GetState(EntityState type, Entity entity);
}