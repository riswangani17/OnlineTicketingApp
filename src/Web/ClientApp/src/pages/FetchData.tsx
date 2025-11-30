import { useEffect, useState } from "react";

type Forecast = {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string | null;
};

function getErrorMessage(err: unknown): string {
  if (err instanceof Error) return err.message;
  if (typeof err === "string") return err;
  return "Unknown error";
}

export default function FetchData() {
  const [loading, setLoading] = useState(true);
  const [forecasts, setForecasts] = useState<Forecast[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      try {
        setLoading(true);
        setError(null);

        const res = await fetch("/weatherforecast");
        if (!res.ok) throw new Error(await res.text());

        const data = (await res.json()) as Forecast[];
        setForecasts(data);
      } catch (err: unknown) {
        setError(getErrorMessage(err));
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  if (loading) return <p><em>Loading...</em></p>;
  if (error) return <pre style={{ color: "crimson" }}>{error}</pre>;

  return (
    <div>
      <h1>Weather forecast</h1>
      <table>
        <thead>
          <tr>
            <th>Date</th>
            <th>Temp (C)</th>
            <th>Temp (F)</th>
            <th>Summary</th>
          </tr>
        </thead>
        <tbody>
          {forecasts.map((f) => (
            <tr key={f.date}>
              <td>{new Date(f.date).toLocaleDateString()}</td>
              <td>{f.temperatureC}</td>
              <td>{f.temperatureF}</td>
              <td>{f.summary ?? "-"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
