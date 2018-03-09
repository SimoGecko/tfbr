// (c) Simone Guggiari 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.Collections.Generic;

namespace BRS.Scripts {
    public interface IDamageable : IComponent {
        void TakeHit(float damage, RaycastHit hit);
        void TakeDamage(float damage);
    }

}
