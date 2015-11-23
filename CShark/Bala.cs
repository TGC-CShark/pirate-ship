using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.CShark
{
    public class Bala
    {
        public TgcSphere bullet;
        public TgcSphere sombra;

        float speed;
        public float verticalSpeed;
        float verticalAcceleration;
        public float anguloRotacion;
        public float anguloElevacion;
        private Canion canion;
        private bool soyPlayer;

        const float RADIO = 4f;

        public Vector3 posicion;

        public Bala(Vector3 pos, float anguloRotacion, float anguloElevacion, Canion canion, bool soyPlayer, float velBala, float g)
        { 
            bullet = new TgcSphere();
            bullet.setColor(Color.Black);
            bullet.Radius = RADIO;
            bullet.Position = pos;
            posicion = pos;
            bullet.LevelOfDetail = 1;
            this.anguloRotacion = anguloRotacion;
            this.anguloElevacion = anguloElevacion;
            this.speed = velBala;
            this.verticalAcceleration = g;
            bullet.updateValues();
            bullet.AutoTransformEnable = false;
            this.canion = canion;
            canion.agregarBalaEnElAire(this);

            sombra = new TgcSphere(RADIO, Color.Black, pos);
            sombra.updateValues();
            sombra.AutoTransformEnable = false;
            sombra.Effect = EjemploAlumno.Instance.efectoSombra;
            sombra.Technique = "SombraBala";
            //sombra.AlphaBlendEnable = true;

            this.soyPlayer = soyPlayer;

            verticalSpeed = speed * (float)Math.Sin(anguloElevacion);
        }

        public void render()
        {
            bullet.render();

            sombra.Effect.SetValue("radioBala", bullet.Radius);
            sombra.Effect.SetValue("posBalaX", posicion.X);
            sombra.Effect.SetValue("posBalaZ", posicion.Z);
            sombra.render();
        }

        public void dispose()
        {
            GuiController.Instance.Logger.log(posicion.ToString());
            GuiController.Instance.Logger.log("bounding: " + bullet.BoundingSphere.Position.ToString());
            canion.eliminarBalaEnElAire(this);
           // bullet.dispose();
        }

        internal void dispararParabolico(float elapsedTime)
        {
            if (posicion.Y >= -50)
            {
                float linearSpeed = speed * (float)Math.Cos(anguloElevacion);

                posicion.X -= Convert.ToSingle(linearSpeed * Math.Sin(anguloRotacion) * elapsedTime);
                posicion.Z -= Convert.ToSingle(linearSpeed * Math.Cos(anguloRotacion) * elapsedTime);

                verticalSpeed -= verticalAcceleration * elapsedTime;
                posicion.Y += Convert.ToSingle(verticalSpeed * elapsedTime);

                Matrix transf = Matrix.Translation(posicion);
                bullet.Transform = Matrix.Scaling(RADIO * 2, RADIO * 2, RADIO * 2) * transf;
                transf.M42 = 0;
                sombra.Transform = Matrix.Scaling(RADIO*2, 1, RADIO*2) * transf;

                bullet.BoundingSphere.moveCenter(posicion - bullet.BoundingSphere.Position);

                bullet.updateValues();
                sombra.updateValues();

                if (soyPlayer)
                {
                    EjemploAlumno.Instance.shipContrincante.verificarDisparo(this);
                }
                else
                {
                    EjemploAlumno.Instance.ship.verificarDisparo(this);
                }
                
            }
            else
            {
                
                this.dispose();
            }
        }

        public void actualizar(float elapsedTime)
        {
            dispararParabolico(elapsedTime);
        }
    }
}
