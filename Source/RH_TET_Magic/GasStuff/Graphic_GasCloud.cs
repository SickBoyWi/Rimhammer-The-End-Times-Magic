using UnityEngine;
using Verse;

namespace TheEndTimes_Magic
{
    public class Graphic_GasCloud : Graphic_Collection
    {
        private const float DistinctAlphaLevels = 128f;

        public override void DrawWorker(
          Vector3 loc,
          Rot4 rot,
          ThingDef thingDef,
          Verse.Thing thing,
          float extraRotation)
        {
            GasCloud gasCloud = (GasCloud)thing;
            Color color = new Color(this.color.r, this.color.g, this.color.b, this.color.a * (Mathf.Round(gasCloud.spriteAlpha * 128f) / 128f));
            Material matSingle = this.subGraphics[(gasCloud.relativeZOrder + thing.Position.x + thing.Position.y) % this.subGraphics.Length].MatSingle;
            Material material = MaterialPool.MatFrom(new MaterialRequest((Texture2D)matSingle.mainTexture, matSingle.shader, color));
            Vector3 drawPos = gasCloud.DrawPos;
            float y = thing.def.altitudeLayer.AltitudeFor() + 0.04285714f * (float)gasCloud.relativeZOrder;
            Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(drawPos.x + gasCloud.spriteOffset.x, y, drawPos.z + gasCloud.spriteOffset.y), Quaternion.AngleAxis(gasCloud.spriteRotation, Vector3.up), new Vector3(this.drawSize.x * gasCloud.spriteScaleMultiplier.x, 0.0f, this.drawSize.y * gasCloud.spriteScaleMultiplier.y));
            UnityEngine.Graphics.DrawMesh(MeshPool.plane10, matrix, material, 0);
        }

        public override string ToString()
        {
            return "GasCloud(path=" + this.path + ", shader=" + (object)this.Shader + ", color=" + (object)this.color + ", variants=" + (object)this.subGraphics.Length + ")";
        }
    }
}
