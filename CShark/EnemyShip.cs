using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    public class EnemyShip : Ship
    {
        const float ROTATION_SPEED = 1f;
        const float VEL_MAXIMA = 400f;
        const float ESCALON_VEL = 0.4f;

        Ship player;
        BarraVidaEnemigo barraDeVidaEnemigo;
        Vector3 distance;

        public EnemyShip(Ship player, Vector3 pos, TgcMesh mesh, Canion canion) : base(pos, mesh, canion) {
            nombre = "ENEMIGO";
            this.player = player;
            anguloRotacion = FastMath.PI / 2;
            iniciarBarra();

        }

        public override void calcularTraslacionYRotacion(float elapsedTime, TerrenoSimple agua, float time)
        {
            distance = player.popa() - this.getPosition();
            Vector3 iaDirectionVersor = this.vectorDireccion();
            iaDirectionVersor.Normalize();
            Vector3 lookAtPopaVersor = new Vector3(distance.X, distance.Y, distance.Z);
            lookAtPopaVersor.Normalize();

            float rotationAngle = FastMath.Acos(Vector3.Dot(iaDirectionVersor, lookAtPopaVersor));
            Vector3 cross = Vector3.Cross(lookAtPopaVersor, iaDirectionVersor);

            Vector3 lastPosition = getPosition();

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

            if (FastMath.Abs(distance.Length()) > 1000)
            {
                movementSpeed = Math.Min(movementSpeed + ESCALON_VEL, VEL_MAXIMA);
            }
            if (FastMath.Abs(distance.Length()) <= 1000)
            {
                if (FastMath.Abs(distance.Length()) > 400) 
                { 
                    movementSpeed = Math.Max(movementSpeed - movementSpeed / distance.Length(), 0);
                }
                else
                {
                    movementSpeed = 0;
                }
                
            }

                movZ -= movementSpeed * FastMath.Cos(anguloRotacion) * elapsedTime;
                movX -= movementSpeed * FastMath.Sin(anguloRotacion) * elapsedTime;
                movY = agua.aplicarOlasA(getPosition(), time).Y + AltoBote / 2;

            administrarColisiones(lastPosition, new Vector3(movX, movY, movZ));
            

            //Cargar valor en UserVar
/*            GuiController.Instance.UserVars.setValue("dir_p", player.vectorDireccion());
            GuiController.Instance.UserVars.setValue("dir_ia", this.vectorDireccion());
            GuiController.Instance.UserVars.setValue("pos_p", player.getPosition());
            GuiController.Instance.UserVars.setValue("pos_ia", this.getPosition());
            GuiController.Instance.UserVars.setValue("popa_p", player.popa());
            GuiController.Instance.UserVars.setValue("popa_ia", this.popa());
            GuiController.Instance.UserVars.setValue("distancia", distance.Length());
            GuiController.Instance.UserVars.setValue("dist_normalizada", lookAtPopaVersor);           
            GuiController.Instance.UserVars.setValue("angulo_rotacion", rotationAngle * 360 / FastMath.PI);
            GuiController.Instance.UserVars.setValue("cross_product", Vector3.Cross(lookAtPopaVersor, iaDirectionVersor));
            GuiController.Instance.UserVars.setValue("cross_product_length", Vector3.Cross(lookAtPopaVersor, iaDirectionVersor).Length());
*/
        }

        public void administrarColisiones(Vector3 lastPosition, Vector3 newPosition) //Le saco la colision con enemigo. Una superclase para ship y enemyship vendria bien, pero paja...
        {

            bool collide = false;
            collide = colisionSkyBox(collide);
            collide = colisionTerreno(collide);
            adaptarMovimientoPorColision(lastPosition, newPosition, collide);

        }

        public override void actualizarCanion(float rotacion, float elapsedTime, Matrix transf)
        {
            mesh.BoundingBox.transform(transf);
            canion.actualizar(anguloRotacion, elapsedTime, transf);
            canion.actualizarSiEsEnemigo(anguloRotacion, elapsedTime, this.movementSpeed, this.distance);
        }

        public override void iniciarBarra()
        {
            barraDeVidaEnemigo = new BarraVidaEnemigo(new Vector2(0, 0), nombre);
        }

        public override void dispose()
        {
            mesh.dispose();
            canion.dispose();
            barraDeVidaEnemigo.dispose();
        }

        public override void renderizar()
        {
            if (tieneVida())
            {
                mesh.render();
                canion.render();
                barraDeVidaEnemigo.render();
            } else
            {
                EjemploAlumno.Instance.estado = EstadoDelJuego.Ganado;
            }

        }

        public override void reducirVida()
        {
            vida -= 1;
            barraDeVidaEnemigo.escalar(porcentajeDeVida());
            GuiController.Instance.Logger.log("Vida contrincante: " + vida.ToString());
        }
    }
}
