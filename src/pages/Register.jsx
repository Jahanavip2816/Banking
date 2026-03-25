import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { registerUser } from "../services/authService";
import "../styles/Register.css";

function Register() {
  const navigate = useNavigate();

  const [form, setForm] = useState({
    username: "",
    email: "",
    password: "",
  });

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      await registerUser({
        username: form.username.trim(),
        email: form.email.trim(),
        password: form.password.trim(),
      });

      alert("Registered successfully ✅");
      navigate("/");
    } catch (err) {
      alert(err.response?.data || "Registration failed ❌");
    }
  };

  return (
    <div className="app-bg">
      <div className="container">
        <div className="left fade-in">

          <h2>Create Account</h2>
          <p className="subtitle">Join ABCBank</p>

          <img src="/images/abc.webp" alt="logo" className="logo" />

          <form onSubmit={handleSubmit}>
            <input
              placeholder="Username"
              onChange={(e)=>setForm({...form,username:e.target.value})}
            />

            <input
              type="email"
              placeholder="Email"
              onChange={(e)=>setForm({...form,email:e.target.value})}
            />

            <input
              type="password"
              placeholder="Password"
              onChange={(e)=>setForm({...form,password:e.target.value})}
            />

            <button type="submit">Register</button>
          </form>

          <div className="links">
            <Link to="/">Already have account? Login</Link>
          </div>

        </div>
      </div>
    </div>
  );
}

export default Register;