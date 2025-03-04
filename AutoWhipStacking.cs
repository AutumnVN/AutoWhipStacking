using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AutoWhipStacking
{
    public class AutoWhipStackingPlayer : ModPlayer
    {
        private bool whipHit = false;
        private uint lastTimeSecondWhipHit = 0;

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (proj.DamageType == DamageClass.SummonMeleeSpeed)
            {
                whipHit = true;
                if (Player.selectedItem == GetSecondWhipSlot())
                {
                    lastTimeSecondWhipHit = Main.GameUpdateCount;
                }
            }
        }

        public override void PostUpdate()
        {
            if (whipHit && Player.itemAnimation == 1)
            {
                if (Player.selectedItem == GetFirstWhipSlot() && Main.GameUpdateCount - lastTimeSecondWhipHit + Player.HeldItem.useTime > 60 * ModContent.GetInstance<AutoWhipStackingConfig>().WhipLoopDelay || Player.selectedItem != GetFirstWhipSlot())
                {
                    SwitchToNextWhip();
                }
                whipHit = false;
            }
        }

        private void SwitchToNextWhip()
        {
            for (int i = 0; i < 10; i++)
            {
                int nextSlot = (Player.selectedItem + i + 1) % 10;
                if (Player.inventory[nextSlot].DamageType == DamageClass.SummonMeleeSpeed)
                {
                    Player.selectedItem = nextSlot;
                    break;
                }
            }
        }

        private int GetFirstWhipSlot()
        {
            for (int i = 0; i < 10; i++)
            {
                if (Player.inventory[i].DamageType == DamageClass.SummonMeleeSpeed)
                {
                    return i;
                }
            }
            return -1;
        }

        private int GetSecondWhipSlot()
        {
            for (int i = 0; i < 10; i++)
            {
                if (Player.inventory[i].DamageType == DamageClass.SummonMeleeSpeed && i != GetFirstWhipSlot())
                {
                    return i;
                }
            }
            return -1;
        }

    }

    public class AutoWhipStackingConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(4)]
        [Range(3, 4)]
        [Slider]
        public int WhipLoopDelay { get; set; }
    }
}
