import React, { useEffect, useState } from "react";
import { api } from "../../service/api";
import { useAuth } from "../../contexts/AuthContext";
import { useNavigate } from "react-router-dom";
import { PieChart, Pie, Cell, Legend, Tooltip, ResponsiveContainer } from "recharts";
import Menu from "../page/Menu";
import "../style/History.css";


const COLORS = ["var(--color4)", "var(--color3)", "var(--color2)"];

function History() {
  const { user } = useAuth();
  const [history, setHistory] = useState([]);
  const [selectedAnalysis, setSelectedAnalysis] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    if (!user || !user.id) {
      navigate("/login");
      return;
    }

    const fetchHistory = async () => {
      try {
        const res = await api.get(`/sentiment/user/${user.id}`);
        setHistory(res.data);
      } catch (err) {
        console.error("Error fetching history:", err);
        setError("Failed to fetch your analysis history.");
      } finally {
        setLoading(false);
      }
    };

    fetchHistory();
  }, [user, navigate]);

  const handleSeeMore = async (analysisId) => {
    try {
      setLoading(true);
      const res = await api.get(`/sentiment/${analysisId}`);
      setSelectedAnalysis({ ...res.data, analysisId });
    } catch (err) {
      console.error("Failed to load detailed analysis", err);
      setError("Failed to load detailed analysis.");
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (analysisId) => {
    if (!window.confirm("Are you sure you want to delete this analysis?")) return;

    try {
      await api.delete(`/sentiment/${analysisId}`);
      setHistory((prev) => prev.filter((item) => item.analysisId !== analysisId));
      setSelectedAnalysis(null);
    } catch (err) {
      console.error("Failed to delete analysis", err);
      setError("Failed to delete the analysis.");
    }
  };

  const renderPieChart = (sentimentDetails) => {
    const data = sentimentDetails.map((s) => ({
      name: s.label,
      value: parseFloat(s.percentage),
    }));

    return (
      <div className="pie-chart-container">
        <ResponsiveContainer width="100%" height={400}>
          <PieChart>
            <Pie
              data={data}
              cx="50%"
              cy="50%"
              /*label={({ name, value }) => `${name}: ${value}%`}*/
              outerRadius="90%"
              dataKey="value"
              labelLine={false}
            >
              {data.map((_, index) => (
                <Cell key={index} fill={COLORS[index % COLORS.length]} />
              ))}
            </Pie>
            <Tooltip formatter={(value) => `${value}%`} />
            <Legend />
          </PieChart>
        </ResponsiveContainer>
      </div>
    );

  };

  if (loading) return <p>Loading...</p>;
  if (error) return <p style={{ color: "var(--error-color)" }}>{error}</p>;

  return (
    <>
      <Menu />
      <div className="history-container">
        <h1>Your History</h1>


        {!selectedAnalysis ? (
          history.length === 0 ? (
            <p>You haven’t performed any analysis yet. Try analyzing some comments!</p>
          ) : (
            <ul className="history-list">
              {history.map((item) => (
                <li key={item.analysisId} className="history-item">
                  {item.source}
                  <br />
                  <button className="btn-secondary" onClick={() => handleSeeMore(item.analysisId)}>
                    See More
                  </button>
                </li>
              ))}
            </ul>
          )
        ) : (
          <div className="analysis-container">
            <h2>Analysis of: </h2>
            <p>{selectedAnalysis.source}</p>
            {selectedAnalysis.sentimentDetails.length === 0 ? (
              <p>No sentiment data available.</p>
            ) : (
              renderPieChart(selectedAnalysis.sentimentDetails)
            )}
            <button className="btn-secondary" onClick={() => setSelectedAnalysis(null)}>
              Return to all
            </button>
            <br />
            <button className="btn-primary" onClick={() => handleDelete(selectedAnalysis.analysisId)}>
              Delete Analysis
            </button>
            <br />
          </div>
        )}
      </div>
    </>
  );
}

export default History;
