import type { PropsWithChildren } from "react";
import NavMenu from "./NavMenu";

export default function Layout({ children }: PropsWithChildren) {
  return (
    <div>
      <NavMenu />
      <main style={{ maxWidth: 960, margin: "0 auto", padding: 16 }}>
        {children}
      </main>
    </div>
  );
}