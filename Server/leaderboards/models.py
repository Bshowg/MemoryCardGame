from django.db import models
import uuid
# Create your models here.
class Leaderboards(models.Model):
    id=models.UUIDField(primary_key=True, default=uuid.uuid4, editable=False)
    name = models.CharField(max_length=50)
    time= models.CharField(max_length=50)
    points=models.CharField(max_length=50)
    number_of_moves=models.CharField(max_length=50)
