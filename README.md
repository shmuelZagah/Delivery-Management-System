# Delivery Management System

A layered, multi-tier delivery management system in C# with a WPF desktop client, built around a pluggable data-access layer, an observer-driven UI update mechanism, and live integration with real mapping/routing services for courier-to-order assignment.

---

https://github.com/user-attachments/assets/bc062854-e5ed-4550-882f-d5041ff86c81

---


## Highlights

- **Swappable persistence backend, chosen at runtime** — the data layer is accessed entirely through an `IDal`/`ICrud<T>` interface. Which concrete implementation runs (`DalList`, an in-memory store, or `DalXml`, file-based XML persistence) is decided at startup by `dal-config.xml` and loaded dynamically via reflection (`Assembly.Load` + `Type.GetType`). Switching storage backends requires no recompilation.
- **Real-world routing, not just straight-line distance** — courier-to-order matching factors in actual travel distance by calling the OSRM routing API (driving/walking profiles) for real road-network distance, with address-to-coordinate geocoding via OpenStreetMap's Nominatim API, and a haversine-formula air-distance fallback if the routing call fails.
- **Asynchronous courier simulation** — `CourierSimulatorAsync` runs an ongoing background process that evaluates open orders against available couriers (respecting each courier's max-range constraint and shipment-type preference) and assigns deliveries automatically, with parallelized data fetches (`Task.WhenAll`) to keep the UI responsive.
- **Observer pattern for live UI sync** — `IObservable` exposes both list-level and per-entity observer registration, so WPF views can subscribe to changes in a specific order/courier or in the collection as a whole and stay in sync without polling.
- **Manual concurrency control** — a lock-free `AsyncMutex` built on `Interlocked.CompareExchange` guards the simulator from re-entrant runs, alongside explicit mutexes for admin config and observer state.
- **Custom Trie data structure** — a standalone, hand-built prefix-tree library (insert/delete/prefix-search/position-tracking) for fast text lookup, built from scratch rather than using a third-party package.
- **Strict layer separation** — Presentation (WPF) → Business Logic → Data Access Facade → concrete Data Access implementation, with each layer talking only to the interface of the layer below it, and dedicated console test-harness projects (`DalTest`, `BITest`) for exercising each layer independently of the UI.

---

## Architecture

```
PL (WPF)
  ↓ talks to
BlApi (IBl, IAdmin, IOrder, ICourier, IObservable)
  ↓ implemented by
BlImplementation
  ↓ talks to
DalApi (IDal, ICrud<T>)         ← Factory resolves implementation via reflection
  ↓ implemented by
DalList (in-memory)   |   DalXml (file-based)

DataBase / Trie         ← standalone prefix-tree library, used for search
```

| Layer | Responsibility |
|---|---|
| `PL` | WPF UI — login, admin console, courier and order management screens |
| `BL` / `BlApi` / `BlImplementation` | Business rules, courier assignment logic, observers, simulation |
| `DalFacade` / `DalApi` | Data contracts (`DO` entities, `ICrud<T>`, `IDal`) — the only thing BL depends on |
| `DalList` / `DalXml` | Concrete, interchangeable storage implementations |
| `DataBase` / `Trie` | Standalone prefix-tree library for fast lookup |
| `DalTest` / `BITest` | Console harnesses for testing DAL/BL logic independently of the UI |

---

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# (.NET) |
| UI | WPF (XAML) |
| Persistence | In-memory list / XML files (pluggable) |
| External services | OSRM (routing), OpenStreetMap Nominatim (geocoding) |
| Concurrency | `async`/`await`, `Task.WhenAll`, `Interlocked` |

---

## Getting Started

### Prerequisites
- Windows
- Visual Studio 2022 (or later) with .NET desktop development workload
- Internet access (for OSRM/Nominatim routing and geocoding calls)

### Build & Run
1. Clone the repository.
2. Open `DeliveryManagementSystem.sln` in Visual Studio.
3. Set `PL` as the startup project.
4. Build and run.
5. To switch the storage backend, edit `xml/dal-config.xml` (`list` or `xml`) before launching.

---

## Project Structure

```
DalFacade/        # DO entities + DAL interfaces (the contract)
DalList/          # in-memory DAL implementation
DalXml/           # XML-file DAL implementation
DalTest/          # console test harness for the DAL
BL/
  ├── BlApi/       # business-logic interfaces
  ├── BlImplementation/
  └── Helpers/     # CourierManager, OrderManager, Tools (routing/geocoding), mutexes
BITest/           # console test harness for the BL
PL/               # WPF application (login, admin, courier, order screens)
DataBase/Trie/    # standalone prefix-tree library
xml/              # runtime config + XML data store
```

---

## Status

Built in deliberate stages (DAL → BL → simulation/observers → UI), with each layer independently testable via its own console harness before being wired into the WPF client.

---

## License

No license file is currently included — all rights reserved by default. Add a license (MIT, Apache 2.0, etc.) if you want others to reuse this code.
