using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;

namespace AlumnoEjemplos.CShark
{
    class MainCamera
    {
        Ship ship;

        public MainCamera(Ship newShip)
        {
            this.ship = newShip;
            ///////////////CONFIGURAR CAMARA ROTACIONAL//////////////////
            //Es la camara que viene por default, asi que no hace falta hacerlo siempre
            GuiController.Instance.RotCamera.Enable = true;
            //Configurar centro al que se mira y distancia desde la que se mira
            
        }

        private void setShip(Ship newShip) {}

        public void actualizar(float elapsedTime){
            apuntarCamara();
        }

        private void apuntarCamara()
        {
            GuiController.Instance.RotCamera.setCamera(ship.position, 100);
        }
    }
}
