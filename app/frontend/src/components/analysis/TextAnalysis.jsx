import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../contexts/AuthContext";
import { api } from "../../service/api";
import Menu from '../page/Menu';
import "../style/Analysis.css"; 

function TextAnalysis() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [inputText, setInputText] = useState("");
  const [analysisData, setAnalysisData] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!user) {
      navigate("/login");
    }
  }, [user, navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setAnalysisData(null);

    try {
      const token = localStorage.getItem("token");
      const response = await api.post(
        "/sentiment/text/analyze",
        JSON.stringify(inputText),
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      setAnalysisData(response.data);
    } catch (error) {
      console.error("Error analyzing text:", error);
      setAnalysisData({ error: "Failed to analyze text." });
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <Menu />
      <div className="analysis-container">
        <h1 className="title">Text Analysis</h1>
        <form onSubmit={handleSubmit} className="analysis-form">
          <textarea
            className="analysis-input"
            rows="10"
            placeholder="Enter your text here..."
            value={inputText}
            onChange={(e) => setInputText(e.target.value)}
          />
          <br />
          <button type="submit" className="btn-primary">
          {loading ? 'Analyzing...' : 'Analyze'}
          </button>
        </form>

        {analysisData && (
          <div className="result">
            <h2>Analysis Result: {analysisData.percentage}% {analysisData.label}</h2>
          </div>
        )}
      </div>
    </>
  );
}


export default TextAnalysis;
