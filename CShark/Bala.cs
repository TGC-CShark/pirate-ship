using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.CShark
{
    class Bala
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
            bullet.dispose();
        }

        internal void dispararParabolico(float elapsedTime)
        {
            posicion.X -= Convert.ToSingle(linearSpeed * Math.Sin(anguloRotacion) * Math.Cos(anguloElevacion) * elapsedTime);
            posicion.Z -= Convert.ToSingle(linearSpeed * Math.Cos(anguloRotacion) * Math.Cos(anguloElevacion) * elapsedTime);

            verticalSpeed -= verticalAcceleration * elapsedTime;
            posicion.Y += Convert.ToSingle(verticalSpeed * Math.Sin(anguloElevacion) * elapsedTime);

            bullet.Transform = Matrix.Scaling(RADIO * 2, RADIO * 2, RADIO * 2) * Matrix.Translation(posicion);
            bullet.updateValues();
        }
    }
}
