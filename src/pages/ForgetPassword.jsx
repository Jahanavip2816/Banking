import { useState } from "react";
import "../styles/Forgot.css";
import "../styles/App.css";

function ForgotPassword() {
  const [email, setEmail] = useState("");
  const [newPass, setNewPass] = useState("");

  const handleReset = (e) => {
    e.preventDefault();
    alert("Password reset successful for " + email);
  };

  return (
    <div className="center-container">
      <div className="card">

        {/* ✅ LOGO */}
        <img src="/images/abc.webp" alt="logo" style={{ width: "70px", display: "block",margin: "0 auto 15px"}}
        />

        <h2>Reset Password</h2>

        <form onSubmit={handleReset}>
          <input
            placeholder="Enter Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />

          <input
            type="password"
            placeholder="New Password"
            value={newPass}
            onChange={(e) => setNewPass(e.target.value)}
          />

          <button type="submit">Reset Password</button>
        </form>
      </div>
    </div>
  );
}

export default ForgotPassword;