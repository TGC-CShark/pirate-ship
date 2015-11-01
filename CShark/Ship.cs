using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
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
    public class Ship
    {
        const float ROTATION_SPEED = 1f;
        const float VEL_MAXIMA = 100f;
        const float ESCALON_VEL = 0.4f;
        const float VIDA_MAX = 5;

        public float movZ;
        public float movY;
        public float movX;
        public float movementSpeed;
        public float anguloRotacion = 0f;

        public Matrix rotacion = Matrix.Identity;
        public Matrix traslacion = Matrix.Identity;

        public float vida;
        public string nombre = "YO";
        public Canion canion;
        private BarraVida barraDeVida;
        public float LargoBote, AnchoBote, AltoBote;

        public TgcViewer.Utils.TgcSceneLoader.TgcMesh mesh;
        static TgcD3dInput input = GuiController.Instance.D3dInput;

        public Ship(Vector3 pos, TgcMesh mesh, Canion canion)
        {
            Vector3 size = new Vector3(15, 10, 30);

            this.mesh = mesh;
            this.mesh.Position = pos;

            movZ = pos.Z;
            movY = pos.Y;
            movX = pos.X;
            traslacion = Matrix.Translation(pos);

            movementSpeed = 0f;

            this.mesh.AutoTransformEnable = false;

            vida = VIDA_MAX;

            // Calcular dimensiones
            Vector3 BoundingBoxSize = mesh.BoundingBox.calculateSize();

            LargoBote = Math.Abs(BoundingBoxSize.Z);
            AnchoBote = Math.Abs(BoundingBoxSize.X);
            AltoBote = Math.Abs(BoundingBoxSize.Y);

            this.canion = canion;

            iniciarBarra();
        }

        public Vector3 getPosition()
        {
            Vector3 pos = new Vector3(mesh.Transform.M41, mesh.Transform.M42, mesh.Transform.M43);
            return pos;
        }

        public virtual void renderizar()
        {
            if (tieneVida())
            {
                mesh.render();
                canion.render();
                barraDeVida.render();
            } else
            {
                EjemploAlumno.Instance.estado = EstadoDelJuego.Perdido;
            }
        }

        public virtual void actualizar(float elapsedTime, TerrenoSimple agua, float time)
        {
            calcularTraslacionYRotacion(elapsedTime, agua, time);

            Matrix transformacion = rotacion * traslacion;

            mesh.Transform = transformacion;
            mesh.BoundingBox.transform(transformacion);
            actualizarCanion(anguloRotacion, elapsedTime, transformacion);
        }

        public virtual void actualizarCanion(float rotacion, float elapsedTime, Matrix transf)
        {
            canion.actualizar(anguloRotacion, elapsedTime, transf);
            canion.actualizarSiEsJugador(anguloRotacion, elapsedTime, transf, movementSpeed);
        }

        public bool tieneVida()
        {
            return vida > 0;
        }

        public virtual void calcularTraslacionYRotacion(float elapsedTime, TerrenoSimple agua, float time)
        {
            Vector3 lastPosition = getPosition();
            Vector3 lastPositionEnemy = EjemploAlumno.Instance.shipContrincante.getPosition();

            if (input.keyDown(Key.Left))
            {
                anguloRotacion -= elapsedTime * ROTATION_SPEED;
                rotacion = Matrix.RotationY(anguloRotacion);
            }

            else if (input.keyDown(Key.Right))
            {
                anguloRotacion += elapsedTime * ROTATION_SPEED;
                rotacion = Matrix.RotationY(anguloRotacion);

            }

            if (input.keyDown(Key.Up))
            {
                movementSpeed = Math.Min(movementSpeed + ESCALON_VEL, VEL_MAXIMA);
            }

            else if (input.keyDown(Key.Down))
            {
                movementSpeed = Math.Max(movementSpeed - ESCALON_VEL, 0);
            }

            movZ -= Convert.ToSingle(movementSpeed * Math.Cos(anguloRotacion) * elapsedTime);
            movX -= Convert.ToSingle(movementSpeed * Math.Sin(anguloRotacion) * elapsedTime);
            movY = agua.aplicarOlasA(getPosition(), time).Y + AltoBote / 2;

      
            administrarColisiones(lastPosition, new Vector3(movX, movY, movZ), lastPositionEnemy);
        }

        public void administrarColisiones(Vector3 lastPosition, Vector3 newPosition, Vector3 lastPositionEnemy)
        {

            bool collide = false;
            collide = colisionSkyBox(collide);
            collide = colisionEnemigo(collide, lastPositionEnemy);
            adaptarMovimientoPorColision(lastPosition, newPosition, collide);

        }

        public bool colisionEnemigo(bool collide, Vector3 lastPositionEnemy)
        {
            TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(mesh.BoundingBox, EjemploAlumno.Instance.shipContrincante.mesh.BoundingBox);

           

            if (result == TgcCollisionUtils.BoxBoxResult.Atravesando || result == TgcCollisionUtils.BoxBoxResult.Adentro)
            {
                collide = true;
                EjemploAlumno.Instance.shipContrincante.traslacion = Matrix.Translation(lastPositionEnemy.X, lastPositionEnemy.Y, lastPositionEnemy.Z);

            }

            return collide;
        }

        public void adaptarMovimientoPorColision(Vector3 lastPosition, Vector3 newPosition, bool collide)
        {
            if (collide)
            {

                movementSpeed = 0;
                traslacion = Matrix.Translation(lastPosition.X, lastPosition.Y, lastPosition.Z);

            }
            else
            {
                traslacion = Matrix.Translation(newPosition.X, newPosition.Y, newPosition.Z);
            }
        }

        public bool colisionSkyBox(bool collide)
        {
            TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(mesh.BoundingBox, EjemploAlumno.Instance.skyBoundingBox.BoundingBox);


            if (result == TgcCollisionUtils.BoxBoxResult.Afuera || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
            {
                collide = true;

            }

            return collide;
        }

        public Vector3 vectorDireccion()
        {
            return new Vector3(- FastMath.Sin(anguloRotacion), 0, - FastMath.Cos(anguloRotacion));
        }

        public Vector3 popa()
        {
            Vector3 offsetPopa = new Vector3(FastMath.Sin(anguloRotacion), 0, FastMath.Cos(anguloRotacion));
            Vector3 popa = getPosition() + offsetPopa * (LargoBote / 2);
            popa.Y = 0;

            return popa;
        }

        public void verificarDisparo(Bala bala)
        {
            if(TgcCollisionUtils.testSphereAABB(bala.bullet.BoundingSphere, mesh.BoundingBox))
            {
                GuiController.Instance.Logger.log("LE DI!");
                reducirVida();
                bala.dispose();
            }
        }

        public virtual void iniciarBarra()
        {
            barraDeVida = new BarraVida(new Vector2(0, 0), nombre);
        }

        public virtual void reducirVida()
        {
            vida -= 1;
            barraDeVida.escalar(porcentajeDeVida());
            GuiController.Instance.Logger.log("Vida contrincante: " + vida.ToString());
        }

        public float porcentajeDeVida()
        {
            return (float)vida / (float)VIDA_MAX;
        }

        public virtual void dispose()
        {
            mesh.dispose();
            canion.dispose();
            //barraDeVida.dispose();
        }
    }
}
