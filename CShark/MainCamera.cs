﻿using Microsoft.DirectX;
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

        const float ALTURA = 500;
        const float DISTANCIA = 1000;

        public MainCamera(Ship newShip)
        {
            this.ship = newShip;
            ///////////////CONFIGURAR CAMARA ROTACIONAL//////////////////
            //Es la camara que viene por default, asi que no hace falta hacerlo siempre
             GuiController.Instance.ThirdPersonCamera.Enable = true;
            
            //Configurar centro al que se mira y distancia desde la que se mira
            GuiController.Instance.ThirdPersonCamera.setCamera(ship.mesh.Position, ALTURA, DISTANCIA);
            

        }

        private void setShip(Ship newShip) {}

        public void actualizar(Vector3 pos){
            GuiController.Instance.ThirdPersonCamera.updateCamera();

            GuiController.Instance.ThirdPersonCamera.Target = pos;
            
        }
    }
}
