using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.CShark
{
    public class Bala
    {
        public TgcSphere bullet;
        public bool visible = false;

        const float linearSpeed = 400;
        public float verticalSpeed;
        public float verticalInitialSpeed = 300;
        float verticalAcceleration = 100f;
        public float anguloRotacion;
        public float anguloElevacion;

        const float RADIO = 4f;

        public Vector3 posicion;

        public Bala(Vector3 pos)
        {

            bullet = new TgcSphere();
            bullet.setColor(Color.Black);
            bullet.Radius = RADIO;
            bullet.Position = pos;
            bullet.LevelOfDetail = 1;
            bullet.updateValues();
            bullet.AutoTransformEnable = false;
        }

        public void render()
        {
            if (visible) { bullet.render(); }
            
        }

        public void dispose()
        {
            GuiController.Instance.Logger.log(posicion.ToString());
            GuiController.Instance.Logger.log("bounding: " + bullet.BoundingSphere.Position.ToString());
            bullet.dispose();
        }

        internal void dispararParabolico(float elapsedTime)
        {
            if (posicion.Y >= -50)
            {
                posicion.X -= Convert.ToSingle(linearSpeed * Math.Sin(anguloRotacion) * Math.Cos(anguloElevacion) * elapsedTime);
                posicion.Z -= Convert.ToSingle(linearSpeed * Math.Cos(anguloRotacion) * Math.Cos(anguloElevacion) * elapsedTime);

                verticalSpeed -= verticalAcceleration * elapsedTime;
                posicion.Y += Convert.ToSingle(verticalSpeed * Math.Sin(anguloElevacion) * elapsedTime);

                bullet.Transform = Matrix.Scaling(RADIO * 2, RADIO * 2, RADIO * 2) * Matrix.Translation(posicion);

                bullet.BoundingSphere.moveCenter(posicion - bullet.BoundingSphere.Position);

                bullet.updateValues();

                EjemploAlumno.Instance.shipContrincante.verificarDisparo(this);
            }
            else
            {
                this.visible = false;
                this.dispose();
            }
        }
    }
}
