using Rotatris.Data;

namespace Rotatris.Systems
{
    /// <summary>
    /// Simulates the same straight-down fall as real gravity, without
    /// mutating game state, to show exactly where the active piece will
    /// land.
    /// </summary>
    public static class GhostSystem
    {
        public static FallingPieceState Compute(StructureState grid, FallingPieceState active, GameConfig config)
        {
            if (!StructureQuerySystem.IsValidCoreAlignment(grid, active.GetCells()))
                return null;

            var sim = active.Clone();

            while (PieceMovementSystem.TryStepDown(grid, sim, config.TargetZoneMinY)) { }

            if (StructureQuerySystem.IsCoreFullyClosed(grid) &&
                !StructureQuerySystem.HasAdjacentNeighbor(grid, sim.GetCells()))
            {
                return null;
            }

            return sim;
        }
    }
}