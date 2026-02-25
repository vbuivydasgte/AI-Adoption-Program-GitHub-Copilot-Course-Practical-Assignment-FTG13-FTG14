import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/useAuth";
import { ROUTES } from "../routes/routesPaths";
import { getErrorMessage } from "../utils/errorHandler";
import { validateLogin } from "../utils/validation";
import styles from "./LoginPage.module.css";

const LoginPage = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.SyntheticEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError("");

    const validation = validateLogin({ username, password });
    if (!validation.isValid) {
      const errorMessages = Object.values(validation.errors).join(", ");
      setError(errorMessages);
      return;
    }

    setLoading(true);

    try {
      await login({ username, password });
      navigate(ROUTES.HOME);
    } catch (err) {
      setError(getErrorMessage(err, "Invalid username or password"));
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.page}>
      <div className={styles.card}>
        <h1 className={styles.title}>Login</h1>

        {error && <div className={styles.error}>{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className={styles.fieldGroup}>
            <label htmlFor="username" className={styles.label}>
              Username
            </label>
            <input
              id="username"
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
              className={styles.input}
            />
          </div>

          <div className={styles.fieldGroupLarge}>
            <label htmlFor="password" className={styles.label}>
              Password
            </label>
            <input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className={styles.input}
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className={`${styles.submit} ${loading ? styles.submitDisabled : ""}`}
          >
            {loading ? "Logging in..." : "Login"}
          </button>
        </form>

        <p className={styles.helperText}>
          Demo Admin: admin / Admin123
          <br />
          Demo Worker: worker / Worker123
        </p>
      </div>
    </div>
  );
};

export default LoginPage;
