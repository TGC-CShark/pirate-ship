﻿using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using TgcViewer;
using TgcViewer.Utils._2D;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.CShark
{
    public class Canion
    {
        const float ELEVACION_MAX = (float)Math.PI / 3 ;
        const float ELEVACION_MIN = 0f;
        const float ELEVACION_INICIAL = (float)Math.PI / 12;
        const float FRECUENCIA_TIRO = 0.5f;
        const float VEL_BALA = 500;
        const float G = 100;

        public TgcMesh meshCanion;
        public int balasRestantes = 50;
        static TgcD3dInput input = GuiController.Instance.D3dInput;
        public List<Bala> balasEnElAire = new List<Bala>();
        public float anguloRotacion;
        public float anguloElevacion = ELEVACION_INICIAL;
        public bool elevacion_visible = false;
        public TgcText2d texto_elevacion;
        bool soyPlayer;

        public Vector3 posicion;

        Timer timer;

        public Ship barco;

        public Canion(Vector3 pos, float offsetShip, TgcMesh mesh, bool soyPlayer)
        {
            
            meshCanion = mesh;
            meshCanion.Position = new Vector3(0, offsetShip, 0) + pos;
            posicion = new Vector3(0, offsetShip, 0) + pos;

            mesh.AutoTransformEnable = false;

            texto_elevacion = new TgcText2d();
            texto_elevacion.Text = toDegrees(anguloElevacion).ToString() + "º";
            texto_elevacion.Color = Color.Gold;
            texto_elevacion.Align = TgcText2d.TextAlign.CENTER;
            texto_elevacion.Position = new Point(200, 100);
            texto_elevacion.Size = new Size(800, 300);
            texto_elevacion.changeFont(new System.Drawing.Font("BlackoakStd", 35, FontStyle.Bold | FontStyle.Italic));

            timer = new Timer(FRECUENCIA_TIRO);

            this.soyPlayer = soyPlayer;
        }

        public void shoot(float elapsedTime, float anguloRotacion, float velBarco)
        {
            new Bala(bocaCanion(), anguloRotacion, anguloElevacion, this, soyPlayer, VEL_BALA, G);
            balasRestantes--;

        }

        public void render()
        {
            meshCanion.render();

            if (elevacion_visible)
            {
                texto_elevacion.render();
            }

        }

        internal void dispose()
        {
            
            meshCanion.dispose();
        }

        internal void actualizar(float anguloRotacion, float elapsedTime, Matrix transformacion)
        {
            this.anguloRotacion = anguloRotacion;
            meshCanion.Transform = calcularMatrizRotacion(transformacion);
            posicion = new Vector3(meshCanion.Transform.M41,meshCanion.Transform.M42,meshCanion.Transform.M43);
        }

        public void actualizarSiEsEnemigo(float anguloRotacion, float elapsedTime, float velBarco, Vector3 distancia)
        {
            this.calcularAnguloElevacion(distancia);
            timer.doWhenItsTimeTo(() => this.shoot(elapsedTime, anguloRotacion, velBarco), elapsedTime);

            for (int i = 0; i < balasEnElAire.Count; i++)
            {
                Bala bala = balasEnElAire[i];
                bala.actualizar(elapsedTime);
                bala.render();
            }
            
        }

        public void actualizarSiEsJugador(float anguloRotacion, float elapsedTime, float velBarco)
        {

            if (input.keyPressed(Key.A))
            {
                elevacion_visible = elevacion_visible ? false : true;
            }

            if (input.keyPressed(Key.Space))
            {
                if (balasRestantes >= 0)
                {
                    shoot(elapsedTime, anguloRotacion, velBarco);
                }
                else
                {
                    
                    if (EjemploAlumno.Instance.shipContrincante.tieneVida())
                    {
                        EjemploAlumno.Instance.estado = EstadoDelJuego.Perdido;
                    }
                }
                
            }

            if (input.keyDown(Key.W))
            {
                incrementarAnguloElevacion(elapsedTime);
            }

            if (input.keyDown(Key.S))
            {
                decrementarAnguloElevacion(elapsedTime);
            }


            //foreach (Bala bala in balasEnElAire)
            for (int i = 0; i < balasEnElAire.Count; i++)
            {
                Bala bala = balasEnElAire[i];
                bala.actualizar(elapsedTime);
                bala.render();
            }
        }

        private void calcularAnguloElevacion(Vector3 distancia)
        {
            Vector3 distLineal = distancia;
            distLineal.Y = 0;

            float h = distancia.Y; //Para calcular el tiro teniendo en cuenta la altura también

            float D = Math.Abs(distLineal.Length());
            float distMax = (float)(Math.Pow(VEL_BALA, 2) * Math.Sin(Math.PI / 2) / G);
            if (D <= distMax)
            {
                anguloElevacion = (float)Math.Asin((D * G) / Math.Pow(VEL_BALA, 2)) / 2;
            }
            else
            {
                anguloElevacion = (float)Math.PI / 4;
            }
        }

        private float toRadians(float grados)
        {
            return grados * (float)Math.PI / 180;
        }

        private float toDegrees(float radianes)
        {
            return radianes * 180 / (float)Math.PI;
        }


        private void decrementarAnguloElevacion(float elapsedTime)
        {
           
            float aux = anguloElevacion;
            anguloElevacion = Math.Max(ELEVACION_MIN, aux-toRadians(1));
            
            texto_elevacion.Text = Convert.ToInt32(toDegrees(anguloElevacion)).ToString() + "º";
        }

        

        private void incrementarAnguloElevacion(float elapsedTime)
        {
            
            float aux = anguloElevacion;
            anguloElevacion = Math.Min(ELEVACION_MAX, aux + toRadians(1));
            
            texto_elevacion.Text = Convert.ToInt32(toDegrees(anguloElevacion)).ToString() + "º";
        }

        public void agregarBalaEnElAire(Bala bala)
        {
            this.balasEnElAire.Add(bala);
        }

        public void eliminarBalaEnElAire(Bala bala)
        {
            this.balasEnElAire.Remove(bala);
        }

        private Matrix calcularMatrizRotacion(Matrix matrizOriginal)
        {
            Matrix matriz = matrizOriginal;
            
            Vector3 pos = obtenerPosicion(matrizOriginal);
            

            //muevo la matriz al 0 (negativo para mandar al centro)
            matriz.Multiply(Matrix.Translation(-pos));
           

            //matriz.Multiply(crearMatrizRotacion(anguloElevacion));
            matriz.Multiply(Matrix.RotationAxis(girar90GradosEnXZ(barco.delante),anguloElevacion-ELEVACION_INICIAL));
            

            //devuelvo al lugar
            matriz.Multiply(Matrix.Translation(pos));
            
            return matriz;
        }

       /* private Matrix crearMatrizTraslacion(Vector3 posicion, Matrix transform)
        {
            Matrix traslacion = Matrix.Identity;
            traslacion.M41 = posicion.X;
            traslacion.M42 = posicion.Y;
            traslacion.M43 = posicion.Z;
            return traslacion;
        }*/

        /*private Matrix crearMatrizRotacion(float anguloElevacion)
        {
            Matrix rotacion = Matrix.Identity;
            rotacion.M22 = (float)Math.Cos(anguloElevacion);
            rotacion.M33 = rotacion.M22;
            rotacion.M23 = (float)Math.Cos(anguloElevacion);
            rotacion.M32 = -rotacion.M23;
            return rotacion;
           

        }*/

        private Vector3 obtenerPosicion(Matrix transform)
        {
            return new Vector3(transform.M41, transform.M42, transform.M43);
        }

        private Vector3 girar90GradosEnXZ(Vector3 vector)
        {
            Vector3 resultado;
            resultado = -Vector3.Cross(vector, new Vector3(0, 1, 0));
            return resultado;
        }

        private Vector3 bocaCanion()
        {
            Vector3 boca;
            boca = barco.delante;
            boca.Y = (float)Math.Sin(anguloElevacion);
            boca += posicion;
            boca.Y += 12;
            return boca;
        }
    }

    /**************************** Timer ****************************************/

    public class Timer
    {
        protected float time;
        protected float frequency;
        public bool itsTime = false;

        public Timer(float frequency)
        {
            this.frequency = frequency;
        }

        public virtual void doWhenItsTimeTo(System.Action whatToDo, float elapsedTime)
        {

            if (itsTime)
            {
                whatToDo();
                itsTime = false;
                GuiController.Instance.Logger.log("dispara enemigo");
            }
            spendTime(elapsedTime);
        }

        protected void spendTime(float elapsedTime)
        {
            if (time < Math.PI * 2)
                time += elapsedTime * (float)Math.PI * frequency;
            else
            {
                itsTime = true;
                this.time = 0;
            }
        }

    }

    public class TimerFinito : Timer
    {
        public bool activo = false;
        private int times = 0;

        public TimerFinito(float frequency) : base(frequency) { }

        public override void doWhenItsTimeTo(System.Action whatToDo, float elapsedTime)
        {
            if (activo)
            {
                //base.doWhenItsTimeTo(whatToDo, elapsedTime);
                if (itsTime)
                {
                    whatToDo();
                    itsTime = false;
                    GuiController.Instance.Logger.log("dispara enemigo");
                    times += 1;
                }
                spendTime(elapsedTime);
                this.apagar();
            }
        }

        private void apagar()
        {
            if (times > 7)
            {
                times = 0;
                activo = false;
            }
        }

       
    }

}
