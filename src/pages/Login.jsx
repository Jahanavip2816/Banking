import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import axios from "axios";
import "../styles/Login.css";
import "../styles/App.css"


function Login() {
  const navigate = useNavigate();

  const [form, setForm] = useState({
    username: "",
    email: "",
    password: "",
  });

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const res = await axios.post(
        "https://localhost:7157/api/auth/login",
        {
          username: form.username.trim(), // ✅ sending both
          email: form.email.trim(),
          password: form.password.trim(),
        }
      );
      <div
  className="container"
  style={{
    backgroundImage: "url('/images/bank3.jpg')",
    backgroundSize: "cover",
    backgroundPosition: "center",
    height: "100vh"
  }}
></div>

      localStorage.setItem("token", res.data.token);

      alert("Login successful ✅");
      navigate("/dashboard");
    } catch (err) {
      console.error(err);

      alert(
        err.response?.data?.message ||
        err.response?.data ||
        "Login failed ❌"
      );
    }
  };

 return (
  <div className="app-bg">
    <div className="container">
      <div className="left fade-in">

        <h2>Sign In</h2>
        <p className="subtitle">Welcome to ABCBank</p>

        <img src="/images/abc.webp" alt="logo" className="logo" />

        <form onSubmit={handleSubmit}>
          <input
            placeholder="Username"
            value={form.username}
            onChange={(e)=>setForm({...form,username:e.target.value})}
          />

          <input
            type="email"
            placeholder="Email"
            value={form.email}
            onChange={(e)=>setForm({...form,email:e.target.value})}
          />

          <input
            type="password"
            placeholder="Password"
            value={form.password}
            onChange={(e)=>setForm({...form,password:e.target.value})}
          />

          <button type="submit">Sign In</button>
        </form>

        <div className="links">
          <Link to="/forgot">Forgot password?</Link> |{" "}
          <Link to="/register">Sign up</Link>
        </div>

      </div>
    </div>
  </div>
);
}

export default Login;