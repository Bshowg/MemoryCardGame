from django.shortcuts import render
from .models import Leaderboards
import json
from django.http import HttpResponseRedirect,HttpResponse, HttpResponseNotFound, JsonResponse
from django.core import serializers
from django.core.serializers.json import DjangoJSONEncoder
from django.views.decorators.csrf import csrf_exempt
# Create your views here.

class LazyEncoder(DjangoJSONEncoder):
    def default(self, obj):
        if isinstance(obj, Leaderboards):
            return str(obj)
        return super().default(obj)

def get_leaderboards(request):
    raw_data = serializers.serialize("python", Leaderboards.objects.all().order_by('-points')[:10])
    actual_data = [d['fields'] for d in raw_data]
    # and now dump to JSON
    output = json.dumps(actual_data)
    return JsonResponse(output, safe=False)

@csrf_exempt
def post_leaderboards(request):
    body_unicode = request.body.decode('utf-8')
    values=body_unicode.split('&')
    l=Leaderboards.objects.create(name=values[0].split('=')[1],points=values[1].split('=')[1],time=values[2].split('=')[1],number_of_moves=values[3].split('=')[1])
    l.save()
    return JsonResponse({"created":"true"})
