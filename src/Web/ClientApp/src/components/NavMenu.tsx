import { NavLink } from "react-router-dom";

const linkStyle = ({ isActive }: { isActive: boolean }) => ({
  marginRight: 12,
  textDecoration: "none",
  fontWeight: isActive ? 700 : 400,
});

export default function NavMenu() {
  return (
    <header style={{ borderBottom: "1px solid #ddd", padding: 12 }}>
      <NavLink to="/" style={linkStyle}>
        Home
      </NavLink>
      <NavLink to="/counter" style={linkStyle}>
        Counter
      </NavLink>
      <NavLink to="/fetch-data" style={linkStyle}>
        Fetch data
      </NavLink>
      <a style={{ marginLeft: 12 }} href="/Identity/Account/Manage">
        Account
      </a>
    </header>
  );
}
