from django.urls import path, include
from . import views
app_name = 'leaderboards'

urlpatterns = [
    path('get/', views.get_leaderboards, name='get'),
    path('post/',views.post_leaderboards,name='post'),
]
