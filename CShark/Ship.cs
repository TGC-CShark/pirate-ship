﻿using Microsoft.DirectX;
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
        const float ESCALON_VEL = 0.7f;
        const float ESCALON_VEL_OLA_BAJADA = 4f;
        const float ESCALON_VEL_OLA_SUBIDA = 1f;
        const float COEF_VEL_OLA_BAJADA = 1.03f;
        const float COEF_VEL_OLA_SUBIDA = 1.03f; 
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

        protected TimerFinito timer;
        protected bool visible = true;

        public Vector3 delante = new Vector3(1,0,0);

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

            timer = new TimerFinito(5);

            // Calcular dimensiones
            Vector3 BoundingBoxSize = mesh.BoundingBox.calculateSize();

            LargoBote = Math.Abs(BoundingBoxSize.Z);
            AnchoBote = Math.Abs(BoundingBoxSize.X);
            AltoBote = Math.Abs(BoundingBoxSize.Y);

            this.canion = canion;
            canion.barco = this;

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
                if (visible)
                {
                    mesh.render();
                    canion.render();
                }
                barraDeVida.render();
            }
            else
            {
                EjemploAlumno.Instance.estado = EstadoDelJuego.Perdido;
            }
        }

        private void alternarVisibilidad()
        {
            visible = visible ? false : true;
        }

        public virtual void actualizar(float elapsedTime, TerrenoSimple agua, float time)
        {
            timer.doWhenItsTimeTo(() => this.alternarVisibilidad(), elapsedTime);

            Vector3 lastPosition = getPosition();

            calcularTraslacionYRotacion(elapsedTime, agua, time, lastPosition);

            Matrix transformacion = rotacion * traslacion;
            delante = new Vector3((float)Math.Sin(anguloRotacion), 0, (float)Math.Cos(anguloRotacion));

            mesh.Transform = transformacion;
            mesh.BoundingBox.transform(transformacion);

            
            alterarVelocidadPorOlas(lastPosition);

            actualizarCanion(anguloRotacion, elapsedTime, transformacion);
        }

        public virtual void actualizarCanion(float rotacion, float elapsedTime, Matrix transf)
        {
            canion.actualizar(anguloRotacion, elapsedTime, transf);
            canion.actualizarSiEsJugador(anguloRotacion, elapsedTime, movementSpeed);
        }


        public virtual void dispose()
        {
            mesh.dispose();
            canion.dispose();
            //barraDeVida.dispose();
        }

        public Vector3 vectorDireccion()
        {
            return new Vector3(-FastMath.Sin(anguloRotacion), 0, -FastMath.Cos(anguloRotacion));
        }

        /************************* PARTES DEL BARCO ****************************/

        public Vector3 proa()
        {
            Vector3 offsetProa = new Vector3(FastMath.Sin(anguloRotacion), 0, FastMath.Cos(anguloRotacion));
            Vector3 proa = getPosition() - offsetProa * (LargoBote / 2);
            proa.Y = 0;

            return proa;
        }

        public Vector3 popa()
        {
            Vector3 offsetPopa = new Vector3(FastMath.Sin(anguloRotacion), 0, FastMath.Cos(anguloRotacion));
            Vector3 popa = getPosition() + offsetPopa * (LargoBote / 2);
            popa.Y = 0;

            return popa;
        }

        /**************** DISPARO *****************************/

        public void verificarDisparo(Bala bala)
        {
            if (TgcCollisionUtils.testSphereAABB(bala.bullet.BoundingSphere, mesh.BoundingBox))
            {
                GuiController.Instance.Logger.log("LE DI!");
                timer.activo = true;
                reducirVida();
                bala.dispose();
            }
        }

        /*************************************** MOVIMIENTO ************************************************/

        public virtual void calcularTraslacionYRotacion(float elapsedTime, TerrenoSimple agua, float time, Vector3 lastPosition)
        {
            
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

        private void alterarVelocidadPorOlas(Vector3 lastPosition)
        {
            if (lastPosition.Y < getPosition().Y)
            {
                movementSpeed = Math.Max(movementSpeed / COEF_VEL_OLA_SUBIDA, 0);//- ESCALON_VEL_OLA_SUBIDA, 0);
            
            } else if (lastPosition.Y > getPosition().Y)
            {
                movementSpeed = Math.Min(movementSpeed * COEF_VEL_OLA_BAJADA, VEL_MAXIMA);//+ ESCALON_VEL_OLA_BAJADA, VEL_MAXIMA);
         
            }

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


        /****************************************** COLISIONES ***************************************************/

        public void administrarColisiones(Vector3 lastPosition, Vector3 newPosition, Vector3 lastPositionEnemy)
        {

            bool collide = false;
            collide = colisionSkyBox(collide);
            collide = colisionEnemigo(collide, lastPositionEnemy);
            collide = colisionTerreno(collide);
            adaptarMovimientoPorColision(lastPosition, newPosition, collide);

        }
        

        //COLISION ENEMIGO
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

        
        //COLISION SKYBOX
        public bool colisionSkyBox(bool collide)
        {
            TgcCollisionUtils.BoxBoxResult result = TgcCollisionUtils.classifyBoxBox(mesh.BoundingBox, EjemploAlumno.Instance.skyBoundingBox.BoundingBox);


            if (result == TgcCollisionUtils.BoxBoxResult.Afuera || result == TgcCollisionUtils.BoxBoxResult.Atravesando)
            {
                collide = true;

            }

            return collide;
        }

        //COLISION TERRENO
        public bool colisionTerreno(bool collide)
        {
            TerrenoSimple terreno = EjemploAlumno.Instance.terrain;
            float YProa = CalcularAltura(proa().X, proa().Z);
            float YPopa = CalcularAltura(popa().X, popa().Z);
            float currentScaleY = EjemploAlumno.Instance.currentScaleY;
            float offset = terreno.Center.Y * currentScaleY;

            if ((Math.Abs(getPosition().Y - offset - YProa) < 1f) && (YPopa - getPosition().Y + offset < 1))
            {
                collide = true;
            }

            return collide;
        }

        public float CalcularAltura(float x, float z)
        {

            float currentScaleXZ = EjemploAlumno.Instance.currentScaleXZ;
            float currentScaleY = EjemploAlumno.Instance.currentScaleY;
            TerrenoSimple terrain = EjemploAlumno.Instance.terrain;

            float largo = currentScaleXZ * 64;
            float pos_i = 64f * (0.5f + x / largo);
            float pos_j = 64f * (0.5f + z / largo);

            int pi = (int)pos_i;
            float fracc_i = pos_i - pi;
            int pj = (int)pos_j;
            float fracc_j = pos_j - pj;

            if (pi < 0)
                pi = 0;
            else
                if (pi > 63)
                    pi = 63;

            if (pj < 0)
                pj = 0;
            else
                if (pj > 63)
                    pj = 63;

            int pi1 = pi + 1;
            int pj1 = pj + 1;
            if (pi1 > 63)
                pi1 = 63;
            if (pj1 > 63)
                pj1 = 63;

            // 2x2 percent closest filtering usual: 
            float H0 = terrain.HeightmapData[pi, pj] * currentScaleY;
            float H1 = terrain.HeightmapData[pi1, pj] * currentScaleY;
            float H2 = terrain.HeightmapData[pi, pj1] * currentScaleY;
            float H3 = terrain.HeightmapData[pi1, pj1] * currentScaleY;
            float H = (H0 * (1 - fracc_i) + H1 * fracc_i) * (1 - fracc_j) +
                      (H2 * (1 - fracc_i) + H3 * fracc_i) * fracc_j;

            return H;
        }


        

        

        /****************************************** VIDA ********************************************/

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

        public bool tieneVida()
        {
            return vida > 0;
        }

       
    }
}
