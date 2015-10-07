using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    class EnemyShip : Ship
    {

        Ship player;

        const float ROTATION_SPEED = 2f;
        const float VEL_MAXIMA = 500f;
        const float ESCALON_VEL = 0.4f;

        public EnemyShip(Ship player, Vector3 pos, TgcMesh mesh, Canion canion) : base(pos, mesh, canion) 
        { 
            nombre = "EnemyShip";
            this.player = player;
        }

        public override void calcularTraslacionYRotacion(float elapsedTime)
        {
            if (GuiController.Instance.D3dInput.keyDown(Key.H))
            {
                anguloRotacion -= elapsedTime * ROTATION_SPEED;
                rotacion = Matrix.RotationY(anguloRotacion);
            }

            else if (GuiController.Instance.D3dInput.keyDown(Key.K))
            {
                anguloRotacion += elapsedTime * ROTATION_SPEED;
                rotacion = Matrix.RotationY(anguloRotacion);

            }

            Vector3 distance = player.popa() - this.getPosition();
            Vector3 iaDirectionVersor = this.vectorDireccion();
            iaDirectionVersor.Normalize();
            Vector3 lookAtPopaVersor = new Vector3(distance.X, distance.Y, distance.Z);
            lookAtPopaVersor.Normalize();

            float rotationAngle = FastMath.Acos(Vector3.Dot(iaDirectionVersor, lookAtPopaVersor));
            Vector3 cross = Vector3.Cross(lookAtPopaVersor, iaDirectionVersor);

            if (cross.Length() > 0.1)
            {
                if (cross.Y > 0.1)
                {
                    anguloRotacion -= elapsedTime * ROTATION_SPEED;
                }
                if (cross.Y < -0.1)
                {
                    anguloRotacion += elapsedTime * ROTATION_SPEED;
                }

                rotacion = Matrix.RotationY(anguloRotacion);
            }

            //anguloRotacion += rotationAngle;

           // rotacion = Matrix.RotationY(rotationAngle);


            //float delta = FastMath.Atan2(diff.X, diff.Z);

            //float correcionGiro = elapsedTime * ROTATION_SPEED ;
            //anguloRotacion = delta < 0 ? anguloRotacion - correcionGiro : anguloRotacion + correcionGiro;
            //anguloRotacion += elapsedTime * ROTATION_SPEED * delta;   
     
            //rotacion = Matrix.RotationY(anguloRotacion);

/*
            rotacion = Matrix.RotationY(player.anguloRotacion);

            if (FastMath.Abs(distance(player)) > 300)
            {
                movementSpeed = VEL_MAXIMA / 4;
            }
            if (FastMath.Abs(distance(player)) < 300)
            {
                movementSpeed = 0;
            }


            movZ -= movementSpeed * FastMath.Cos(anguloRotacion) * elapsedTime;
            movX -= movementSpeed * FastMath.Sin(anguloRotacion) * elapsedTime;
            traslacion = Matrix.Translation(diff.X * 5, 0, diff.Z * 5);
*/            
            //traslacion = Matrix.Translation(diff.X * 5, 0, diff.Z * 5);
            //Cargar valor en UserVar
            GuiController.Instance.UserVars.setValue("dir_p", player.vectorDireccion());
            GuiController.Instance.UserVars.setValue("dir_ia", this.vectorDireccion());
            GuiController.Instance.UserVars.setValue("pos_p", player.getPosition());
            GuiController.Instance.UserVars.setValue("pos_ia", this.getPosition());
            GuiController.Instance.UserVars.setValue("popa_p", player.popa());
            GuiController.Instance.UserVars.setValue("popa_ia", this.popa());
            GuiController.Instance.UserVars.setValue("distancia", distance);
            GuiController.Instance.UserVars.setValue("dist_normalizada", lookAtPopaVersor);           
            GuiController.Instance.UserVars.setValue("angulo_rotacion", rotationAngle * 360 / FastMath.PI);
            GuiController.Instance.UserVars.setValue("cross_product", Vector3.Cross(lookAtPopaVersor, iaDirectionVersor));
            GuiController.Instance.UserVars.setValue("cross_product_length", Vector3.Cross(lookAtPopaVersor, iaDirectionVersor).Length());
        }

        public override void actualizarCanion(float rotacion, float elapsedTime, Vector3 newPosition)
        {

        }

    }
}
