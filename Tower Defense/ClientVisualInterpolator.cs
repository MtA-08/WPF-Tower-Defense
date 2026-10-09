using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace TowerDefense
{
    // Nur im UI-Thread verwenden. Diese Klasse aendert ausschliesslich
    // DrawingVisual.Offset, niemals die vom Host empfangenen Spieldaten.
    internal sealed class ClientVisualInterpolator
    {
        private sealed class Movement
        {
            public Vector Start;
            public Vector Target;
            public double Elapsed;
        }

        private readonly double duration;
        private readonly Dictionary<DrawingVisual, Movement> movements =
            new Dictionary<DrawingVisual, Movement>();

        public ClientVisualInterpolator(double duration)
        {
            if (duration <= 0 || double.IsNaN(duration) || double.IsInfinity(duration))
                throw new ArgumentOutOfRangeException(nameof(duration));

            this.duration = duration;
        }

        // Einmal pro empfangenem Objekt aufrufen.
        public void SetTarget(DrawingVisual visual, Vector target, bool snapImmediately)
        {
            Movement movement;
            if (!movements.TryGetValue(visual, out movement))
            {
                movement = new Movement
                {
                    Start = target,
                    Target = target,
                    Elapsed = duration
                };

                movements.Add(visual, movement);
                visual.Offset = target;
                return;
            }

            // Eine Pause zeigt sofort den bestaetigten, angehaltenen Zustand.
            if (snapImmediately)
            {
                movement.Start = target;
                movement.Target = target;
                movement.Elapsed = duration;

                if (visual.Offset != target)
                    visual.Offset = target;

                return;
            }

            // Identische Snapshots duerfen eine laufende Bewegung nicht
            // immer wieder von vorne beginnen lassen.
            if (movement.Target == target)
                return;

            // Bei einem fruehen Snapshot von der gerade sichtbaren Position
            // fortsetzen, damit die Grafik nicht rueckwaerts springt.
            movement.Start = visual.Offset;
            movement.Target = target;
            movement.Elapsed = 0;
        }

        // Jeden Render-Frame mit echter, NICHT mit TimeScale multiplizierter
        // Zeit aufrufen. Bei Pause oder Verbindungsende ruft der Controller
        // diese Methode nicht auf.
        public void Update(double deltaTime)
        {
            if (deltaTime <= 0 || double.IsNaN(deltaTime) || double.IsInfinity(deltaTime))
                return;

            foreach (KeyValuePair<DrawingVisual, Movement> entry in movements)
            {
                Movement movement = entry.Value;

                if (movement.Elapsed >= duration)
                    continue;

                movement.Elapsed = Math.Min(movement.Elapsed + deltaTime, duration);
                double progress = movement.Elapsed / duration;

                Vector position = progress >= 1
                    ? movement.Target
                    : movement.Start + (movement.Target - movement.Start) * progress;

                if (entry.Key.Offset != position)
                    entry.Key.Offset = position;
            }
        }

        // Vor dem Entfernen des jeweiligen DrawingVisual aufrufen.
        public void Remove(DrawingVisual visual)
        {
            movements.Remove(visual);
        }
    }
}
