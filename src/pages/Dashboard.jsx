import { useState, useEffect } from "react";
import API from "../services/api";
import "../styles/Dashboard.css";

function Dashboard() {
  const [page, setPage] = useState("home");
  const [selectedAccount, setSelectedAccount] = useState(null);

  const [transactions, setTransactions] = useState([]);
  const [pageNo, setPageNo] = useState(1);

  const [transaction, setTransaction] = useState({
  amount: "",
  description: "",
  password: ""
});

const [filter, setFilter] = useState({
  page: 1,
  size: 5,
  type: "",
  sort: "desc"
});

  const [form, setForm] = useState({
    accountHolderName: "",
    phone: "",
    password: "",
  });

  // ================= ACCOUNT =================

  const openViewAccount = async () => {
    try {
      const res = await API.get("/accounts/my");
      setSelectedAccount(res.data);
      setPage("view");
    } catch {
      alert("No account ❌");
    }
  };

  const createAccount = async () => {
  try {
    await API.post("/accounts", form);
    alert("Account Created ✅");
    openViewAccount();
  } catch (err) {
    if (err.response?.status === 400 || err.response?.status === 409) {
      alert("Account already exists ❌");
      setPage("create");
    } else {
      alert(err.response?.data || "Error ❌");
    }
  }
};

  const openTransactions = async () => {
  try {
    const res = await API.get("/accounts/my");
    setSelectedAccount(res.data);
    setPage("transactions");
  } catch {
    alert("Create account first ❌");
  }
};
  // ================= TRANSACTIONS =================

  const depositMoney = async () => {
  if (!transaction.amount || transaction.amount <= 0) {
    alert("Amount is required and should be greater than 0 ❌");
    return;
  }

  if (!transaction.password) {
    alert("Password is required ❌");
    return;
  }

  try {
    await API.post("/transactions/deposit", {
      accountId: selectedAccount.id,
      amount: parseFloat(transaction.amount),
      description: transaction.description,
      password: transaction.password
    });

    alert("Deposit successful ✅");

    setTransaction({ amount: "", description: "", password: "" });
    setPage("transactions");

  } catch (err) {
    alert(err.response?.data || "Deposit failed ❌");
  }
};

  const withdrawMoney = async () => {
  if (!transaction.amount || transaction.amount <= 0) {
    alert("Amount is required and should be greater than 0 ❌");
    return;
  }

  if (!transaction.password) {
    alert("Password is required ❌");
    return;
  }

  try {
    await API.post("/transactions/withdraw", {
      accountId: selectedAccount.id,
      amount: parseFloat(transaction.amount),
      description: transaction.description,
      password: transaction.password
    });

    alert("Withdraw successful ✅");

    setTransaction({ amount: "", description: "", password: "" });
    setPage("transactions");

  } catch (err) {
    alert(err.response?.data || "Withdraw failed ❌");
  }
};

const fetchTransactions = async (page = 1) => {
  try {
    const res = await API.get(
      `/transactions/account/${selectedAccount.id}/paged`,
      {
        params: {
          page: page,
          size: filter.size,
          type: filter.type || "",
          sort: filter.sort
        }
      }
    );

    console.log("Transactions API:", res.data); // ✅ DEBUG

    setTransactions(res.data?.data || res.data); // handle both formats
    setPageNo(page);

  } catch (err) {
    console.error(err);
    alert("Error loading transactions ❌");
  }
};

  // ================= REPORTS =================

  const openReports = async () => {
    try {
      const res = await API.get("/accounts/my");
      setSelectedAccount(res.data);
      setPage("reports");
    } catch {
      alert("No account ❌");
    }
  };

  const viewPDF = () => {
    window.open(
      `https://localhost:7157/api/reports/transactions/${selectedAccount.id}/pdf-receipt`,
      "_blank"
    );
  };

  const downloadPDF = () => {
    window.open(
      `https://localhost:7157/api/reports/transactions/${selectedAccount.id}/pdf`,
      "_blank"
    );
  };

  const downloadCSV = () => {
    window.open(
      `https://localhost:7157/api/reports/transactions/${selectedAccount.id}/csv`,
      "_blank"
    );
  };

  const goHome = () => setPage("home");

  const logout = () => {
    localStorage.removeItem("token");
    window.location.href = "/login";
  };

  return (
    <div className="dashboard">

      {/* HEADER */}
      <div className="header">
        <h1>ABCBank</h1>
        <button className="logout-btn" onClick={logout}>Logout</button>
      </div>

      <div className="content">

        {/* HOME */}
        {page === "home" && (
          <div className="card-grid">
            <div className="card" onClick={() => setPage("create")}>Create your account</div>
            <div className="card" onClick={openViewAccount}>View your account </div>
            <div className="card" onClick={openTransactions}>Do Transactions</div>
            <div className="card" onClick={openReports}>Reports</div>
          </div>
        )}

        {/* create account */}
        {page === "create" && (
  <div className="create-account-box">
    <h2>Create Account</h2>

    <label>Name</label>
    <input
      placeholder="Enter full name"
      value={form.accountHolderName}
      onChange={(e) =>
        setForm({ ...form, accountHolderName: e.target.value })
      }
    />

    <label>Phone</label>
    <input
      placeholder="Enter phone number"
      value={form.phone}
      onChange={(e) => setForm({ ...form, phone: e.target.value })}
    />

    <label>Password</label>
    <input
      type="password"
      placeholder="Enter password"
      value={form.password}
      onChange={(e) => setForm({ ...form, password: e.target.value })}
    />

    <button onClick={createAccount} className="primary-btn">
      Create Account
    </button>

    <button onClick={goHome} className="back-btn">
      Back
    </button>
  </div>
)}

{/* View */}
{page === "view" && selectedAccount && (
  <div className="view-account-box">
    <h2>Account Details</h2>
    <table className="account-table">
      <tbody>
        <tr>
          <td>Account ID</td>
          <td>{selectedAccount.id}</td>
        </tr>
        <tr>
          <td>Name</td>
          <td>{selectedAccount.accountHolderName}</td>
        </tr>
        <tr>
          <td>Email</td>
          <td>{selectedAccount.email}</td>
        </tr>
        <tr>
          <td>Phone</td>
          <td>{selectedAccount.phone}</td>
        </tr>
        <tr>
          <td>Balance</td>
          <td>₹{selectedAccount.balance}</td>
        </tr>
        <tr>
          <td>Created Date</td>
          <td>{new Date(selectedAccount.createdDate).toLocaleString()}</td>
        </tr>
      
          
      </tbody>
    </table>
    <button onClick={goHome} className="back-btn">Back</button>
  </div>
)}
    
    {page === "transactions" && (
  <div>
    <div className="card-grid">
      <div className="card" onClick={() => setPage("deposit")}>Deposit</div>
      <div className="card" onClick={() => setPage("withdraw")}>Withdraw</div>
      <div className="card" onClick={() => {
        setPage("viewTransactions");
        fetchTransactions(1);
      }}>
        View Transactions
      </div>
    </div>

    <button onClick={goHome} className="back-btn">Back</button>
  </div>
)}

        {page === "deposit" && (
  <div className="form-container">
    <h2>Deposit Money</h2>

    <label>Amount</label>
    <input type="number" required value={transaction.amount} onChange={(e) =>
    setTransaction({ ...transaction, amount: e.target.value })
      }
    />

    <label>Description</label>
    <input
      type="text"
      placeholder="Enter description"
      value={transaction.description}
      onChange={(e) =>
        setTransaction({ ...transaction, description: e.target.value })
      }
    />

    <label>Password</label>
    <input type="password" required value={transaction.password} onChange={(e) =>
    setTransaction({ ...transaction, password: e.target.value })
    }
    />

    <button onClick={depositMoney} className="primary-btn">
      Deposit
    </button>

    <button onClick={() => setPage("transactions")} className="back-btn">
      Back
    </button>
  </div>
)}

{page === "withdraw" && (
  <div className="form-container">
    <h2>Withdraw Money</h2>

    <label>Amount</label>
    <input type="number" required value={transaction.amount} onChange={(e) =>
    setTransaction({ ...transaction, amount: e.target.value })
      }
    />

    <label>Description</label>
    <input type="text" value={transaction.description} onChange={(e) =>
        setTransaction({ ...transaction, description: e.target.value })
      }
    />

    <label>Password</label>
    <input type="password" required value={transaction.password} onChange={(e) =>
    setTransaction({ ...transaction, password: e.target.value })
    }
    />

    <button onClick={withdrawMoney} className="primary-btn">
      Withdraw
    </button>

    <button onClick={() => setPage("transactions")} className="back-btn">
      Back
    </button>
  </div>
)}

{page === "viewTransactions" && (
  <div className="view-account-box">
    <h2>Transaction History</h2>

    {transactions.length === 0 ? (
      <p style={{ textAlign: "center" }}>No transactions found ❌</p>
    ) : (
      <table className="account-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Amount</th>
            <th>Type</th>
            <th>Description</th>
            <th>Date</th>
          </tr>
        </thead>
        <tbody>
          {transactions.map((t) => (
            <tr key={t.id}>
              <td>{t.id}</td>
              <td>₹{t.amount}</td>
              <td>{t.type}</td>
              <td>{t.description}</td>
              <td>{new Date(t.date).toLocaleString()}</td>
            </tr>
          ))}
        </tbody>
      </table>
    )}

    <div className="pagination">
      <button disabled={pageNo === 1} onClick={() => fetchTransactions(pageNo - 1)}>
        Prev
      </button>
      <button onClick={() => fetchTransactions(pageNo + 1)}>
        Next
      </button>
    </div>

    <button onClick={() => setPage("transactions")} className="back-btn">
      Back
    </button>
  </div>
)}

  {page === "reports" && selectedAccount && (
  <div className="reports-box">
    <h2>Reports</h2>

    <div className="reports-grid">
      <div className="report-card" onClick={viewPDF}>
        📄 Download Receipt
      </div>

      <div className="report-card" onClick={downloadPDF}>
        📑 Download PDF
      </div>

      <div className="report-card full-width" onClick={downloadCSV}>
        📊 Download CSV
      </div>
    </div>

    <button onClick={goHome} className="back-btn">
      Back
    </button>
  </div>
)}
  
      </div>
    </div>
  );
}

export default Dashboard;
