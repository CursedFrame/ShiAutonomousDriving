using UnityEditor;
using UnityEngine;

namespace GleyTrafficSystem
{
    public class SetupWindowBase : Editor, ISetupWindow
    {
        protected const int BOTTOM_SPACE = 70;
        protected const int TOGGLE_DIMENSION = 15;
        protected const int BUTTON_DIMENSION = 70;
        protected const int SCROLL_SPACE = 20;
        protected const int TOGGLE_WIDTH = 168;

        protected Vector2 scrollPosition = Vector2.zero;

        private WindowType windowType;
        private string windowTitle;
        private string tutorialLink;
        private bool enabled;
        private bool showBack;
        private bool showTitle;
        private bool showTop;
        private bool showScroll;
        private bool showBottom;
        

        public virtual ISetupWindow Initialize(WindowProperties windowProperties)
        {
            windowType = windowProperties.type;
            windowTitle = windowProperties.name;
            showBack = windowProperties.showBack;
            showTitle = windowProperties.showTitle;
            showTop = windowProperties.showTop;
            showScroll = windowProperties.showScroll;
            showBottom = windowProperties.showBottom;
            tutorialLink = windowProperties.tutorialLink;
            enabled = true;
            SettingsWindow.BlockClicks(windowProperties.blockClicks);
            return this;
        }


        public string GetWindowTitle()
        {
            return windowTitle;
        }


        public bool DrawInWIndow(float width, float height)
        {
            if (showBack)
            {
                Navigation();
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (showTitle)
            {
                WindowTitle();
            }
            if (showTop)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                TopPart();
                EditorGUILayout.EndVertical();
            }

            if (showScroll)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                ScrollPart(width, height);
                EditorGUILayout.EndVertical();
            }

            if (showBottom)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                BottomPart();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
            return enabled;
        }


        public virtual void DrawInScene()
        {

        }


        public virtual void MouseMove(Vector3 mousePosition)
        {

        }


        public virtual void LeftClick(Vector3 mousePosition)
        {

        }


        public virtual void RightClick(Vector3 mousePosition)
        {

        }


        public virtual void UndoAction()
        {

        }


        void CloseWindow()
        {
            enabled = false;
        }

        public virtual void DestroyWindow()
        {

        }


        public WindowType GetWindowType()
        {
            return windowType;
        }


        private void Navigation()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (GUILayout.Button("<< Back", GUILayout.Width(BUTTON_DIMENSION)))
            {
                CloseWindow();
            }
            EditorGUILayout.LabelField(NavigationRuntimeData.GetBackPath() + GetWindowTitle());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }


        protected virtual void WindowTitle()
        {
            GUIStyle style = EditorStyles.label;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(GetWindowTitle(), style);
            style.fontStyle = FontStyle.Normal;
            style.alignment = TextAnchor.MiddleLeft;
        }


        protected virtual void TopPart()
        {

        }


        protected virtual void ScrollPart(float width, float height)
        {

        }


        protected virtual void BottomPart()
        {
            if(GUILayout.Button("View Tutorial"))
            {
                Application.OpenURL(tutorialLink);
            }
        }
    }
}
