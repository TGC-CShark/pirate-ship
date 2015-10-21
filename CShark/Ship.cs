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
        const float VEL_MAXIMA = 500f;
        const float ESCALON_VEL = 0.4f;
        const float VIDA_MAX = 5;

        public float movZ;
        public float movY;
        public float movX;
        public  float movementSpeed;
        public float anguloRotacion = 0f;

        public Matrix rotacion = Matrix.Identity;
        public Matrix traslacion = Matrix.Identity;

        public float vida;
        public string nombre = "YO";
        public Canion canion;
        private BarraVida barraDeVida;      
        private float LargoBote, AnchoBote, AltoBote;

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
            Vector3 pos = new Vector3(movX, movY, movZ);
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
            actualizarCanion(anguloRotacion, elapsedTime, transformacion);           
        }

        public virtual void actualizarCanion(float rotacion, float elapsedTime, Matrix transf)
        {
            canion.actualizar(anguloRotacion, elapsedTime, transf);
            canion.actualizarSiEsJugador(anguloRotacion, elapsedTime, transf);
        }

        public bool tieneVida()
        {
            return vida > 0;
        }

        public virtual void calcularTraslacionYRotacion(float elapsedTime, TerrenoSimple agua, float time)
        {
            if (input.keyDown(Key.Left))
            {
                anguloRotacion -= elapsedTime * ROTATION_SPEED;
                rotacion = Matrix.RotationY(anguloRotacion);
            }

            else if (input.keyDown(Key.Right) )
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
            //movY = agua.aplicarOlasA(mesh.Position, time).Y + AltoBote/2;

            traslacion = Matrix.Translation(movX, 0, movZ);
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
