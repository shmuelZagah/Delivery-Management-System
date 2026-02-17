# 📌 רשימת בונוסים ותוספות בפרויקט
use 'show markdown preview'

לנוחיות הבודק/ת, להלן טבלה המפרטת את התוספות שבוצעו בפרויקט ואת מיקומן בקוד.


## 👤 משתמשים למערכת

השתמש בפרטים הבאים כדי להתחבר למערכת:

### 🛡️ כניסת מנהל (Admin)
* **שם משתמש:** `222222222`
* **סיסמה:** `admin100`

### 📦 כניסת שליח (Courier)
* **שם משתמש:** `234567890`
* **סיסמה:** `1234567890`


## 🏗️ שכבות נתונים ולוגיקה (DAL & BL)


| נושא / בונוס | סטטוס | מיקום בקוד (לחץ לפתיחה)| שורות | הערות |ניקוד|
| :--- | :---: | :---: | :--- | :---: | :---: |
| **בדיקות קלט (TryParse)** | ✅ | [`BITest/Program.cs`](./BITest/Program.cs) / [`DalTest/Program.cs`](./DalTest/Program.cs) |58,92... / 449,470... |שימוש ב-`int.TryParse` למניעת קריסות בקליטת מספרים בתפריטים. | 1|
| **Singleton & Thread Safe** | ✅ | [`DalList.cs`](./DalList/DalList.cs) / [`DalXml.cs`](./DalXml/DalXml.cs) |רואים מול העיניים | שימוש ב-`lock` ובמופע יחיד (Instance) למניעת התנגשויות. |2|
| **סיסמא** | ✅ | [`PasswordManager.cs`](./DalFacade/DalApi/IConfig.cs) / [`PasswordCourier`](./DalFacade/DO/Courier.cs) |7 / 12 | שימוש ב-`lock` ובמופע יחיד (Instance) למניעת התנגשויות. | 2|


## 🎨 ממשק משתמש (WPF / UI)

| נושא / בונוס | סטטוס | מיקום בקוד (לחץ לפתיחה)| שורות | הערות |ניקוד|
| :--- | :---: |:---: | :---: | :---: |  :---: |
| **ערכת נושא (Theme)** | ✅ | [`Style1.xaml`](./PL/Helpers/Styles/Style1.xaml) / [`App.xaml`](./PL/App.xaml) |510 / 11| מחובר דרך `App.xaml` לכל האפליקציה. | 1|
| **טריגר תכונות** | ✅ | [`Style1.xaml`](./PL/Helpers/Styles/Style1.xaml) |268| שינוי צבע בעת מעבר עכבר (`IsMouseOver`). | 1|
| **Control Template** | ✅ | [`Style1.xaml`](./PL/Helpers/Styles/Style1.xaml) |468| עיצוב מותאם אישית לכפתורים (פינות עגולות). |1|
| **הסתרת סיסמה** | ✅ | [`LoginWindow.xaml`](./PL/LoginWindow.xaml) |64| שימוש בפקד `PasswordBox`. | 1|
| **לחיצה על Enter ככפתור** | ✅ | [`LoginWindow.xaml`](./PL/LoginWindow.xaml) |70| שימוש בתכונה `IsDefault="True"`. | 1|
| **behavior** | ✅ | [`Behavior.cs`](./PL/Helpers/Behavior/InputValidators.cs) [`Example for use`](./PL/Order/OrderWindow.xaml) |// 170| אימות קלט ברמת ה-XAML. | 1|
| **תכונות מצורפות (Attached)** | ✅ | [`Behavior.cs`](./PL/Helpers/Behavior/InputValidators.cs) |18,21| אימות קלט ברמת ה-XAML. | 1|
| **גרפיקה (Shapes)** | ✅ | [`LoginWindow.xaml`](./PL/LoginWindow.xaml) / [`Icon`](./PL/Helpers/Items/Icon.xaml) |67,77| שימוש באלמנטים גרפיים. | 1|
| **כפתור מחיקה חכם** | ✅ | [`Converters`](./PL/Helpers/Converters/Visibility.cs) / [`Cancel in orders`](./PL/Order/OrderListWindow.xaml) |// 122 | הסתרה באמצעות `BooleanToVisibilityConverter`. | 2|
| **קיבוץ הזמנות** | ✅ | [`Order List`](./PL/Order/OrderListWindow.xaml.cs)  /  [`xaml`](./PL/Order/OrderListWindow.xaml) | 154 / 75 | grouping functionality. | 2 |
| **אייקון** | ✅ | [`Style->Icon`](./PL/Helpers/Styles/Style1.xaml)| 154 / 75 | אייקון בסרגל משימות וסרגל של החלון. | 1|
| **ולידציה** | ✅ | [`inpute user name`](./PL/LoginWindow.xaml.cs) | 66 | בדיקה שהקלט לא ריק | 1|
| **שינוי צבעי ולידציה** | ✅ | [`Color chang`](./PL/LoginWindow.xaml) / [`The convertor`](./PL/Helpers/Converters/InputNumericToColorConverter.cs)| 78 / כל העמוד | שינוי צבע הקלט בהתאם למצב הוולידציה | 1|
| **טריגר נתונים** | ✅ | [`data trigger`](./PL/LoginWindow.xaml) |  67 | מעלים את הטקסט אם התחילו להזין | 1|
| **טריגר אירועים** | ✅ | [`Style1.xaml`](./PL/Helpers/Styles/Style1.xaml)  |179| כאשר חלון נפתח יוצר אפקט חמוד | 1|


