using UnityEngine;

public class SemiPistolPickUp : ItemPickUp
{
    public string weaponName = "SemiPistol"; // Nombre del arma
    public int ammoAmount = 15; // Cantidad de munición que otorga

    protected override void OnPickUp(Collider player)
    {

    }

    private void ActivateWeapon(Collider player, string weaponName)
    {
        // Aquí puedes implementar la lógica para activar el arma en el jugador

    }
}
