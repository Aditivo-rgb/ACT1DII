using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputManager")]
public class InputManagerSO : ScriptableObject
{
    Controls misControles;
    public event Action OnSaltar;
    public event Action <Vector2> OnMover;
    public event Action OnDisparar;
    public event Action OnRecargar;
    public event Action OnPausar;
    private void OnEnable()
    {
        //Esto es para suscribirse, lanzas la se�al al resto de actores de unity de que est�s apretando el bot�n que sea para que pasen co
        misControles = new Controls();
        misControles.Gameplay.Enable();
        misControles.Gameplay.Saltar.started += Saltar;
         misControles.Gameplay.Disparar.started += Disparar;
        misControles.Gameplay.Recargar.started += Recargar;
        //lo anterior son botones, esto es el joystick
        misControles.Gameplay.Mover.performed += Mover;
        misControles.Gameplay.Mover.canceled += Mover;
        misControles.Gameplay.Pausar.started += Pausar;
        


        Debug.Log("EstoyListo");
    }

    public void ActivarControles()
    {
        misControles.Gameplay.Enable();
    }
    public void DesactivarControles()
    {
        misControles.Gameplay.Disable();
    }
    private void Pausar(InputAction.CallbackContext obj)
    {
        OnPausar?.Invoke();
    }

    private void Recargar(InputAction.CallbackContext obj)
    {
        OnRecargar?.Invoke();
    }

    private void Disparar(InputAction.CallbackContext obj)
    {
        OnDisparar?.Invoke();
    }

    private void Mover(InputAction.CallbackContext ctx)
    {
        OnMover?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void Saltar(InputAction.CallbackContext ctx)
    {
        OnSaltar?.Invoke();
    }
}
