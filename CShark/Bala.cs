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
        float verticalSpeed;
        float verticalAcceleration;
        public float anguloRotacion;

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
            posicion.X -= Convert.ToSingle(linearSpeed * Math.Sin(anguloRotacion) * elapsedTime);
            posicion.Z -= Convert.ToSingle(linearSpeed * Math.Cos(anguloRotacion) * elapsedTime);

            bullet.Transform = Matrix.Scaling(RADIO * 2, RADIO * 2, RADIO * 2) * Matrix.Translation(posicion);
            bullet.updateValues();
        }
    }
}
