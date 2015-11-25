using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Input;

namespace AlumnoEjemplos.CShark
{
    class MainCamera
    {
        Ship ship;
        TgcThirdPersonCamera camera;
        TgcD3dInput input = GuiController.Instance.D3dInput;

        const float ALTURA = 500;
        const float DISTANCIA = 1000;
        const float ROTATION_SPEED = 0.05f;

        float zoom;
        float wheelPos;
        Vector3 objetive;

        Boolean apuntando = false;
        Vector3 apuntado = new Vector3(0,0,0);
        float ALTURA_APUNTANDO=0;
        float DISTANCIA_APUNTANDO=100;

        public MainCamera(Ship newShip)
        {
            this.ship = newShip;
            camera = GuiController.Instance.ThirdPersonCamera;
            camera.Enable = true;

            wheelPos = input.WheelPos;
            objetive = ship.mesh.Position;
            //Configurar centro al que se mira y distancia desde la que se mira
            camera.setCamera(objetive, ALTURA, DISTANCIA);
        }

        private void setShip(Ship newShip)
        {
            this.ship = newShip;
        }

        public void actualizar(Vector3 pos)
        {
            if (input.keyPressed(Key.P))
            {
                if (!apuntando)
                {
                    apuntando = true;
                    apuntado = new Vector3(0, 50, -50);
                    camera.OffsetForward = DISTANCIA_APUNTANDO;
                    camera.OffsetHeight = ALTURA_APUNTANDO;


                }
                else
                {
                    
                    apuntando = false;
                    apuntado = new Vector3(0, 0, 0);

                    camera.setCamera(pos, ALTURA, DISTANCIA);

                    

                }
            }
            if (apuntando)
            {
                camera.RotationY = ship.anguloRotacion;
            }
            //Solo rotar si se esta aprentando el boton izq del mouse
            if (input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                
                if(!apuntando)
                {
                    camera.rotateY(-input.XposRelative * ROTATION_SPEED);
                }
            }

            if (input.keyDown(Key.LeftShift))
            {
                camera.OffsetHeight += 5;
                camera.OffsetForward += 5;
            }

            if (input.keyDown(Key.LeftControl))
            {
                camera.OffsetHeight -= 5;
                camera.OffsetForward -= 5;
            }

            if (input.WheelPos != wheelPos)
            {
                zoom = input.WheelPos - wheelPos < 0 ? 20f : -20F;
                camera.OffsetHeight += zoom;
                camera.OffsetForward += zoom;
            }


            Vector3 aux = ship.delante;
            aux.Y = 0;
            camera.Target = pos+new Vector3(0,apuntado.Y,0)+(apuntado.Z*aux);





            camera.updateCamera();

        }
    }
}
